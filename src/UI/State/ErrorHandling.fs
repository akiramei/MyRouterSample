namespace UI.State

open Domain.Errors
open Domain.ValueObjects.Localization
open Application.ErrorTranslation
open Application.Services.ErrorMessageService
open UI.State.Messages
open Common.Platform

/// UI層のエラーハンドリング
module ErrorHandling =

    /// エラーメッセージを取得
    let getErrorMessage (error: IError) (language: Language) = getUserMessage error language

    /// UIイベントハンドラ用のエラー処理
    let handleError (error: IError) (language: Language) (dispatch: AppMsg -> unit) =
        // まずドメインエラーに変換
        let domainError = ErrorTranslationService.translateToDomainError error

        // ユーザー向けメッセージを取得
        let message = getErrorMessage domainError language

        // 開発環境では追加情報をログに出力
        if PlatformServices.Environment.isDevelopment then
            let debugInfo = getDebugInfo domainError
            PlatformServices.Logging.debug (sprintf "Error details: %s" debugInfo)

        // UIにエラーを表示
        dispatch (ShowError message)