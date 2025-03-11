# This sample is for learning

# F# エラー処理戦略 実践ガイド

このガイドでは、本プロジェクトで採用しているF#のエラー処理戦略について説明します。インターフェースベースのエラー型とRailway Oriented Programmingを組み合わせた、SOLID原則に準拠したエラー処理アプローチを採用しています。

## 1. エラー処理の基本概念

本プロジェクトのエラー処理は以下の原則に基づいています：

1. **エラーのインターフェース化** - 共通インターフェースによる一貫したエラー処理
2. **層ごとの明確な責任分担** - 各層が独自のエラータイプを定義
3. **Railway Oriented Programming** - 成功/失敗を明示的に型で表現
4. **多言語対応** - ユーザーの言語設定に合わせたエラーメッセージ
5. **コンテキスト情報の提供** - デバッグに役立つ詳細情報

## 2. エラー型のアーキテクチャ

エラー処理の中心となるのが以下の型構造です：

```fsharp
// ドメイン層：共通エラーインターフェース
type IError =
    /// エラーのカテゴリー
    abstract Category: string
    /// エラーコード（識別子）
    abstract Code: string
    /// ユーザー向けエラーメッセージ
    abstract UserMessage: string
    /// デバッグ用コンテキスト情報
    abstract Context: Map<string, string> option
    /// コンテキスト情報を追加したエラーを返す
    abstract WithContext: Map<string, string> -> IError

// 結果型（Railway Oriented Programming用）
type Result<'T> =
    | Success of 'T
    | Failure of IError
```

各層は独自のエラータイプを定義し、IErrorインターフェースを実装します：

```fsharp
// ドメイン層のエラー
type DomainErrorDetails =
    | ValidationError of field: string * message: string
    | NotFoundError of entityType: string * id: string
    | BusinessRuleViolation of rule: string * details: string

type DomainError =
    { Details: DomainErrorDetails
      ErrorContext: Map<string, string> option }
    interface IError with
        // インターフェース実装
        
// UI層のエラー
type UIErrorDetails =
    | MissingInput of fieldName: string
    | InvalidSelection of selection: string
    | FormError of message: string

type UIError =
    { Details: UIErrorDetails
      ErrorContext: Map<string, string> option }
    interface IError with
        // インターフェース実装

// インフラストラクチャ層のエラー
type InfrastructureErrorDetails =
    | NetworkError of message: string
    | AuthenticationError of message: string
    | AuthorizationError of message: string
    | SystemError of message: string

type InfrastructureError =
    { Details: InfrastructureErrorDetails
      ErrorContext: Map<string, string> option }
    interface IError with
        // インターフェース実装
```

## 3. 基本的な使い方

### 3.1 エラーの作成

各層は自分の責任範囲のエラーを作成するヘルパー関数を持っています：

```fsharp
// ドメインエラーの作成
let validationError = 
    ErrorHelpers.asDomainError 
        (ValidationError("username", "ユーザー名は必須です")) 
        None

// UIエラーの作成
let missingInputError = 
    UIErrorHelpers.missingInput "password"
        
// インフラエラーの作成
let networkError = 
    InfrastructureErrorHelpers.networkError "サーバーに接続できません"
```

### 3.2 Railway Oriented Programming

Railway演算子を使うと、処理の成功/失敗パスを優雅に表現できます：

```fsharp
// バリデーション関数
let validateUsername username =
    if System.String.IsNullOrWhiteSpace username then
        let error = ErrorHelpers.asDomainError 
            (ValidationError("username", "ユーザー名は必須です")) 
            None
        Failure error
    else
        Success username

// 複数のバリデーションを連結
let validateUser username password =
    validateUsername username
    >>= (fun username -> validatePassword password 
                        >>= (fun password -> Success (username, password)))
```

### 3.3 エラーの翻訳

異なる層からのエラーを適切に翻訳するためのサービスを用意しています：

```fsharp
// インフラエラーをドメインエラーに変換
let translateInfrastructureError (error: InfrastructureError) : IError =
    match error.Details with
    | NetworkError message ->
        ErrorHelpers.asDomainError 
            (BusinessRuleViolation("connection", "サーバーに接続できません"))
            (Some (Map ["original_error", message]))
```

### 3.4 エラーの表示

UIでエラーを表示する場合：

```fsharp
match error with
| Some error ->
    Daisy.alert [
        alert.error
        prop.className "mb-4"
        prop.children [
            Html.span [ prop.text error.UserMessage ]
        ]
    ]
| None -> Html.none
```

多言語対応が必要な場合：

```fsharp
let errorMessage = 
    match ResourceKeyMapper.getErrorResourceKey error with
    | Some resourceKey -> getText userLanguage resourceKey
    | None -> error.UserMessage
```

## 4. メッセージハンドリングパターン

Elmishアプリケーションでのエラー処理：

```fsharp
// エラー表示メッセージ
| ShowError message ->
    { model with
        ErrorDisplay = { IsVisible = true; Message = Some message } },
    Cmd.none
    
// エラークリア
| ClearError ->
    { model with
        ErrorDisplay = { IsVisible = false; Message = None } },
    Cmd.none
```

ドメイン固有の処理では、エラーを適切に変換します：

```fsharp
| UserDataError error ->
    { model with IsLoading = false }, 
    Cmd.ofMsg (ShowUserProfileError (ErrorTranslationService.translateToDomainError error).UserMessage)
```

## 5. ベストプラクティス

### 5.1 早期バリデーション

入力値は早期に検証し、バリデーションエラーを即座に返します：

```fsharp
let validateInput input =
    if String.IsNullOrWhiteSpace input then
        Failure (ErrorHelpers.asDomainError 
            (ValidationError("input", "入力は必須です")) None)
    else if input.Length > 100 then
        Failure (ErrorHelpers.asDomainError 
            (ValidationError("input", "入力は100文字以内にしてください")) None)
    else
        Success input
```

### 5.2 エラーの集約

複数のバリデーションを行う場合、エラーを集約することも検討してください：

```fsharp
let validateForm username password email =
    let errors = []
    let errors = 
        if String.IsNullOrWhiteSpace username then 
            ValidationError("username", "ユーザー名は必須です") :: errors 
        else errors
        
    let errors = 
        if String.IsNullOrWhiteSpace password then 
            ValidationError("password", "パスワードは必須です") :: errors 
        else errors
        
    // エラーがあれば失敗、なければ成功
    match errors with
    | [] -> Success (username, password, email)
    | errors -> 
        let formError = ErrorHelpers.asDomainError 
            (BusinessRuleViolation("form", "入力内容に問題があります")) 
            (Some (Map [("errors", string errors.Length)]))
        Failure formError
```

### 5.3 コンテキスト情報の活用

デバッグに役立つコンテキスト情報を提供しましょう：

```fsharp
let handleApiError (ex: System.Exception) url =
#if FABLE_COMPILER
    // Fable環境向け簡略化バージョン
    InfrastructureErrorHelpers.fromFableException ex
        |> InfrastructureErrorHelpers.withRequestInfo url 500
#else
    // 標準.NET環境向け
    InfrastructureErrorHelpers.fromException ex
        |> InfrastructureErrorHelpers.withRequestInfo url 500
#endif
```

### 5.4 層間の依存関係に注意

ドメイン層は他の層に依存してはいけません。エラーに関連する処理もこのルールに従います：

```fsharp
// 良い例 - ドメイン層は自己完結している
namespace Domain.Errors
module ErrorHelpers =
    let asDomainError details context = ...

// 悪い例 - ドメイン層がUI層やインフラ層に依存している
namespace Domain.Errors
module ErrorHelpers =
    open UI.Errors // ドメイン層が他の層に依存 - 避けるべき
    let someFunction error = ...
```

## 6. SOLID原則との適合性

このエラー処理アプローチは以下のSOLID原則に準拠しています：

### 単一責任の原則 (SRP)
各エラータイプは明確に定義された役割を持ち、それぞれの層は自分が管理すべきエラーだけを処理します。

### 開放閉鎖の原則 (OCP)
新しいエラータイプを追加する場合、既存のコードを修正する必要はありません。各層が独自のエラー実装を提供できます。

### リスコフの置換原則 (LSP)
すべてのエラータイプは`IError`インターフェースを実装しており、どのエラータイプも`IError`として扱うことができます。

### インターフェース分離の原則 (ISP)
`IError`インターフェースはエラー処理に必要な最小限のメンバーのみを定義しています。

### 依存関係逆転の原則 (DIP)
すべての層がドメイン層で定義された`IError`インターフェースに依存し、ドメイン層は他の層に依存しません。

## 7. 結論

このエラー処理戦略を統一的に活用することで、保守性が高く、型安全なコードが実現できます。エラーは明示的に扱われ、予期しないエラーの伝播を防止します。また、ユーザー体験も向上し、適切なエラーメッセージによってユーザーは何が問題であるかを理解できます。

SOLID原則やクリーンアーキテクチャの考え方に基づくこのアプローチは、アプリケーションの成長に伴い、その真価を発揮します。