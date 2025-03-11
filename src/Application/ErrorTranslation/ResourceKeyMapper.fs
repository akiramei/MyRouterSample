namespace Application.ErrorTranslation

open Domain.Errors
open Shared.I18n.TranslationService

/// エラーからリソースキーへのマッピングサービス
module ResourceKeyMapper =

    /// エラーからリソースキーを抽出するヘルパー
    let getErrorResourceKey (error: IError) : ResourceKey option =
        match error with
        | :? DomainError as domainError ->
            match domainError.Details with
            | Domain.Errors.ValidationError(field, _) ->
                match field.ToLower() with
                | "username" -> Some UsernameRequired
                | "password" -> Some PasswordRequired
                | "language" -> Some LanguageRequired
                | _ -> Some ValidationError
            | _ -> None
        | _ -> None
