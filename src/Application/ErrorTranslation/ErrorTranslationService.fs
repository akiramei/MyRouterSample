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
            ErrorHelpers.createBusinessRuleErrorWithParams
                "connectivity"
                "error.network.connection"
                (Map [ ("endpoint", endpoint); ("message", message) ])

        | AuthenticationError message ->
            ErrorHelpers.createBusinessRuleErrorWithParams
                "authentication"
                "error.authentication.failed"
                (Map [ ("message", message) ])

        | AuthorizationError(resource, action) ->
            ErrorHelpers.createBusinessRuleErrorWithParams
                "authorization"
                "error.authorization.denied"
                (Map [ ("resource", resource); ("action", action) ])

        | SystemError(code, message) ->
            ErrorHelpers.createBusinessRuleErrorWithParams
                "system"
                "error.system.generic"
                (Map [ ("code", code); ("message", message) ])

    /// UIエラーをドメインエラーに変換
    let translateUIError (error: UIError) : IError =
        match error.Details with
        | MissingInput field ->
            ErrorHelpers.createValidationErrorWithParams field "error.field.required" (Map [ ("field", field) ])

        | InvalidSelection selection ->
            ErrorHelpers.createValidationErrorWithParams "selection" "error.invalid.selection" (Map [ ("selection", selection) ])

        | FormError(messageKey, parameters) -> 
            ErrorHelpers.createValidationErrorWithParams "form" messageKey parameters

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
            ErrorHelpers.createBusinessRuleErrorWithParams
                "unknown"
                "error.unknown"
                (Map [ ("code", error.Code); ("message", error.MessageKey) ])