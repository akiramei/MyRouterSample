namespace Domain.Errors

/// 共通エラーインターフェース
/// すべてのエラーはこのインターフェースを実装する
type IError =
    /// エラーコード - システム識別用
    abstract Code: string
    /// エラーキー - メッセージ翻訳用
    abstract MessageKey: string
    /// メッセージパラメータ - メッセージのプレースホルダ置換用
    abstract MessageParams: Map<string, string>
    /// 追加コンテキスト - ログやデバッグ用
    abstract Context: Map<string, string> option
    /// コンテキスト情報を追加したエラーを返す
    abstract WithContext: Map<string, string> -> IError

/// ドメインエラーの詳細
type DomainErrorDetails =
    | ValidationError of field: string * messageKey: string * parameters: Map<string, string>
    | NotFoundError of entityType: string * id: string
    | BusinessRuleViolation of rule: string * messageKey: string * parameters: Map<string, string>

/// ドメインエラー実装
type DomainError =
    { Details: DomainErrorDetails
      ErrorContext: Map<string, string> option }

    interface IError with
        member this.Code =
            match this.Details with
            | ValidationError _ -> "DOM-VAL-001"
            | NotFoundError _ -> "DOM-NF-001"
            | BusinessRuleViolation _ -> "DOM-BIZ-001"

        member this.MessageKey =
            match this.Details with
            | ValidationError(_, key, _) -> key
            | NotFoundError(_, _) -> "error.entity.not.found"
            | BusinessRuleViolation(_, key, _) -> key

        member this.MessageParams =
            match this.Details with
            | ValidationError(field, _, params) -> Map.add "field" field params
            | NotFoundError(entity, id) -> Map [ ("entity", entity); ("id", id) ]
            | BusinessRuleViolation(_, _, params) -> params

        member this.Context = this.ErrorContext

        member this.WithContext contextMap =
            let newContext =
                match this.ErrorContext with
                | Some existingContext -> Some(Map.fold (fun acc k v -> Map.add k v acc) existingContext contextMap)
                | None -> Some contextMap

            { this with ErrorContext = newContext } :> IError

// F#標準のResult型を使用
type Result<'TSuccess> = Result<'TSuccess, IError>
