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
        | NetworkError(endpoint, message) ->
            ErrorHelpers.businessRuleWithParams
                "connectivity"
                "error.network.connection"
                (Map [ ("endpoint", endpoint); ("message", message) ])

        | AuthenticationError message ->
            ErrorHelpers.businessRuleWithParams
                "authentication"
                "error.authentication.failed"
                (Map [ ("message", message) ])

        | AuthorizationError(resource, action) ->
            ErrorHelpers.businessRuleWithParams
                "authorization"
                "error.authorization.denied"
                (Map [ ("resource", resource); ("action", action) ])

        | SystemError(code, message) ->
            ErrorHelpers.businessRuleWithParams
                "system"
                "error.system.generic"
                (Map [ ("code", code); ("message", message) ])

    /// UIエラーをドメインエラーに変換
    let translateUIError (error: UIError) : IError =
        match error.Details with
        | MissingInput field ->
            ErrorHelpers.validationWithParams field "error.field.required" (Map [ ("field", field) ])

        | InvalidSelection selection ->
            ErrorHelpers.validationWithParams "selection" "error.invalid.selection" (Map [ ("selection", selection) ])

        | FormError(messageKey, params) -> ErrorHelpers.validationWithParams "form" messageKey params

    /// 任意のエラーをドメインエラーに変換
    let translateToDomainError (error: IError) : IError =
        match error with
        | :? DomainError ->
            // すでにドメインエラーの場合はそのまま返す
            error

        | :? InfrastructureError as infraError ->
            // インフラエラーをドメインエラーに変換
            translateInfrastructureError infraError

        | :? UIError as uiError ->
            // UIエラーをドメインエラーに変換
            translateUIError uiError

        | _ ->
            // 未知のエラー型の場合は汎用的なエラーに変換
            ErrorHelpers.businessRuleWithParams
                "unknown"
                "error.unknown"
                (Map [ ("code", error.Code); ("message", error.MessageKey) ])
