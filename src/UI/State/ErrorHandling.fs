namespace UI.State

open Elmish
open Domain.Errors
open Domain.ValueObjects.User
open Application.ErrorTranslation
open Application.Services.ErrorMessageService

/// UI層のエラーハンドリング
module ErrorHandling =

    /// エラーメッセージを取得
    let getErrorMessage (error: IError) (language: Language) = getUserMessage error language

    /// UIイベントハンドラ用のエラー処理
    let handleError (error: IError) (language: Language) (dispatch: Types.Msg -> unit) =
        // まずドメインエラーに変換
        let domainError = ErrorTranslationService.translateToDomainError error

        // ユーザー向けメッセージを取得
        let message = getErrorMessage domainError language

        // デバッグモードでは追加情報をログに出力
#if DEBUG
        let debugInfo = getDebugInfo domainError
        Browser.Dom.console.error (sprintf "Error details: %s" debugInfo)
#endif

        // UIにエラーを表示
        dispatch (Types.ShowError message)
