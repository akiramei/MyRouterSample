namespace Domain.Errors

/// 共通エラーインターフェース
/// すべてのエラーはこのインターフェースを実装する
type IError =
    /// エラーのカテゴリー
    abstract Category: string
    /// エラーコード（識別子）
    abstract Code: string
    /// ユーザー向けエラーメッセージ
    abstract UserMessage: string
    /// デバッグや詳細情報のためのコンテキスト
    abstract Context: Map<string, string> option
    /// コンテキスト情報を追加したエラーを返す
    abstract WithContext: Map<string, string> -> IError

/// Railway Oriented Programming のための結果型
type Result<'TSuccess> =
    | Success of 'TSuccess
    | Failure of IError

/// ドメインエラーの詳細
type DomainErrorDetails =
    | ValidationError of field: string * message: string
    | NotFoundError of entityType: string * id: string
    | BusinessRuleViolation of rule: string * details: string

/// ドメインエラー実装
type DomainError =
    { Details: DomainErrorDetails
      ErrorContext: Map<string, string> option }

    interface IError with
        member this.Category = "Domain"

        member this.Code =
            match this.Details with
            | ValidationError _ -> "DOM-VAL-001"
            | NotFoundError _ -> "DOM-NF-001"
            | BusinessRuleViolation _ -> "DOM-BIZ-001"

        member this.UserMessage =
            match this.Details with
            | ValidationError(field, message) -> sprintf "入力エラー: %s - %s" field message
            | NotFoundError(entity, id) -> sprintf "%s (ID: %s) が見つかりません" entity id
            | BusinessRuleViolation(rule, details) -> sprintf "ビジネスルール違反: %s - %s" rule details

        member this.Context = this.ErrorContext

        member this.WithContext contextMap =
            let newContext =
                match this.ErrorContext with
                | Some existingContext -> Some(Map.fold (fun acc k v -> Map.add k v acc) existingContext contextMap)
                | None -> Some contextMap

            { this with ErrorContext = newContext } :> IError
