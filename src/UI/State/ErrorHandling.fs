namespace UI.State

open Elmish
open Domain.Errors
open Domain.ValueObjects.User

/// UI層のエラーハンドリング
module ErrorHandling =
    open Shared.I18n.TranslationService
    
    /// エラー型から翻訳リソースキーへの変換
    let toResourceKey (error: Error) =
        match error.Category with
        | DomainError ->
            match error.Details with
            | :? DomainErrorDetails as details ->
                match details with
                // 完全修飾名を使用して名前空間の衝突を回避
                | Domain.Errors.ValidationError (field, _) -> 
                    match field.ToLower() with
                    | "username" -> Some Shared.I18n.TranslationService.UsernameRequired
                    | "password" -> Some Shared.I18n.TranslationService.PasswordRequired
                    | "language" -> Some Shared.I18n.TranslationService.LanguageRequired
                    | _ -> Some Shared.I18n.TranslationService.ValidationError
                | _ -> None
            | _ -> None
        | UIError ->
            match error.Details with
            | :? UIErrorDetails as details ->
                match details with
                | MissingInput field -> 
                    match field.ToLower() with
                    | "username" -> Some Shared.I18n.TranslationService.UsernameRequired
                    | "password" -> Some Shared.I18n.TranslationService.PasswordRequired
                    | "language" -> Some Shared.I18n.TranslationService.LanguageRequired
                    | _ -> Some Shared.I18n.TranslationService.ValidationError
                | _ -> None
            | _ -> None
        | _ -> None
    
    /// エラーメッセージを多言語対応で取得
    let getErrorMessage (error: Error) (language: Language) =
        match toResourceKey error with
        | Some resourceKey -> getText language resourceKey
        | None -> ErrorHelpers.toUserMessage error
        
    /// UIイベントハンドラ用のエラー処理
    let handleError (error: Error) (language: Language) (dispatch: Types.Msg -> unit) =
        let message = getErrorMessage error language
        match error.Category with
        | DomainError | UIError ->
            // UI関連エラーの場合はユーザーにメッセージを表示
            dispatch (Types.ShowError message)
        | InfrastructureError ->
            // インフラエラーの場合はログに記録し、必要に応じて別の処理
            #if DEBUG
            Browser.Dom.console.error($"システムエラー: {message}")
            #endif
            dispatch (Types.ShowError "システムエラーが発生しました。管理者に連絡してください。")