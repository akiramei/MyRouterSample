# This sample is for learning

# F# エラー処理戦略 実践ガイド

このガイドでは、本プロジェクトで採用しているF#のエラー処理戦略について説明します。階層的なエラー型とRailway Oriented Programmingを組み合わせた、保守性と拡張性に優れたエラー処理アプローチを採用しています。

## 1. エラー処理の基本概念

本プロジェクトのエラー処理は以下の原則に基づいています：

1. **エラーの階層化** - カテゴリごとにエラーを整理
2. **Railway Oriented Programming** - 成功/失敗を明示的に型で表現
3. **多言語対応** - ユーザーの言語設定に合わせたエラーメッセージ
4. **コンテキスト情報の提供** - デバッグに役立つ詳細情報

## 2. エラー型のアーキテクチャ

エラー処理の中心となるのが以下の型構造です：

```fsharp
// エラーカテゴリ
type ErrorCategory =
    | DomainError
    | UIError
    | InfrastructureError

// 汎用エラー型
type Error = 
    { Category: ErrorCategory
      Details: obj
      Context: Map<string, string> option }
      
// 結果型（Railway Oriented Programming用）
type Result<'T> =
    | Success of 'T
    | Failure of Error
```

各エラーカテゴリには具体的な詳細型があります：

```fsharp
// ドメインエラーの詳細
type DomainErrorDetails =
    | ValidationError of field: string * message: string
    | NotFoundError of entityType: string * id: string
    | BusinessRuleViolation of rule: string * details: string

// UIエラーの詳細
type UIErrorDetails =
    | MissingInput of fieldName: string
    | InvalidSelection of selection: string
    | FormError of message: string

// インフラストラクチャエラーの詳細
type InfrastructureErrorDetails =
    | NetworkError of message: string
    | AuthenticationError of message: string
    | AuthorizationError of message: string
    | SystemError of message: string
```

## 3. 基本的な使い方

### 3.1 エラーの作成

エラーを作成するには、エラーヘルパー関数を使用します：

```fsharp
// ドメインエラーの作成
let validationError = 
    ErrorHelpers.asDomainError 
        (ValidationError("username", "ユーザー名は必須です")) 
        None

// UIエラーの作成
let missingInputError = 
    ErrorHelpers.asUIError 
        (MissingInput("password")) 
        None
        
// インフラエラーの作成
let networkError = 
    ErrorHelpers.asInfrastructureError 
        (NetworkError("サーバーに接続できません")) 
        (Some (Map [("url", "https://api.example.com")]))
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

### 3.3 エラーの表示

UIでエラーを表示する場合：

```fsharp
match error with
| Some error ->
    Daisy.alert [
        alert.error
        prop.className "mb-4"
        prop.children [
            Html.span [ prop.text (ErrorHelpers.toUserMessage error) ]
        ]
    ]
| None -> Html.none
```

多言語対応が必要な場合：

```fsharp
let errorMessage = 
    match UI.State.ErrorHandling.toResourceKey error with
    | Some resourceKey -> getText userLanguage resourceKey
    | None -> ErrorHelpers.toUserMessage error
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

ドメイン固有の処理では、エラーをメッセージに変換します：

```fsharp
// ユーザープロファイルのエラー処理例
| UserDataError error ->
    { model with IsLoading = false }, 
    Cmd.ofMsg (ShowUserProfileError (ErrorHelpers.toUserMessage error))
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
    let context = Map [
        ("url", url)
        ("statusCode", "???")
        ("timestamp", System.DateTime.Now.ToString())
        ("exception", ex.GetType().Name)
    ]
    
    ErrorHelpers.asInfrastructureError 
        (NetworkError(ex.Message)) 
        (Some context)
```

## 6. 結論

このエラー処理戦略を統一的に活用することで、保守性が高く、型安全なコードが実現できます。エラーは明示的に扱われ、予期しないエラーの伝播を防止します。また、ユーザー体験も向上し、適切なエラーメッセージによってユーザーは何が問題であるかを理解できます。

新たな機能を追加する際には、このパターンに従って一貫したエラー処理を実装してください。
