namespace Application.ErrorTranslation

open Domain.Errors
open UI.Errors
open Infrastructure.Errors

/// エラー翻訳サービス
/// 外部層のエラーをドメイン層で理解できるエラーに変換する
module ErrorTranslationService =

    /// インフラストラクチャエラーをドメインエラーに変換
    let translateInfrastructureError (error: InfrastructureError) : IError =
        match error.Details with
        | NetworkError message ->
            ErrorHelpers.asDomainError
                (BusinessRuleViolation("connection", "サーバーに接続できません"))
                (Some(Map [ "original_error", message; "category", "infrastructure" ]))

        | AuthenticationError message ->
            ErrorHelpers.asDomainError
                (BusinessRuleViolation("authentication", "認証に失敗しました"))
                (Some(Map [ "original_error", message; "category", "infrastructure" ]))

        | AuthorizationError message ->
            ErrorHelpers.asDomainError
                (BusinessRuleViolation("authorization", "権限がありません"))
                (Some(Map [ "original_error", message; "category", "infrastructure" ]))

        | SystemError message ->
            ErrorHelpers.asDomainError
                (BusinessRuleViolation("system", "システムエラーが発生しました"))
                (Some(Map [ "original_error", message; "category", "infrastructure" ]))

    /// UIエラーをドメインエラーに変換
    let translateUIError (error: UIError) : IError =
        match error.Details with
        | MissingInput field ->
            ErrorHelpers.asDomainError
                (ValidationError(field, sprintf "%sは必須です" field))
                (Some(Map [ "category", "ui" ]))

        | InvalidSelection selection ->
            ErrorHelpers.asDomainError
                (ValidationError("selection", sprintf "選択された値 '%s' は無効です" selection))
                (Some(Map [ "category", "ui" ]))

        | FormError message ->
            ErrorHelpers.asDomainError (ValidationError("form", message)) (Some(Map [ "category", "ui" ]))

    /// 任意のエラーをドメインエラーに変換
    let translateToDomainError (error: IError) : IError =
        match error with
        | :? DomainError -> error // 既にドメインエラーの場合はそのまま返す
        | :? InfrastructureError as infraError -> translateInfrastructureError infraError
        | :? UIError as uiError -> translateUIError uiError
        | _ ->
            // 未知のエラー型の場合は汎用的なドメインエラーに変換
            ErrorHelpers.asDomainError
                (BusinessRuleViolation("unknown", "不明なエラーが発生しました"))
                (Some(Map [ "original_category", error.Category; "original_code", error.Code ]))
