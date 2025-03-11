namespace UI.State

open Elmish
open Domain.Errors
open Domain.ValueObjects.User
open Application.ErrorTranslation

/// UI層のエラーハンドリング
module ErrorHandling =
    open Shared.I18n.TranslationService

    /// エラー型から翻訳リソースキーへの変換
    let toResourceKey (error: IError) =
        ResourceKeyMapper.getErrorResourceKey error

    /// エラーメッセージを多言語対応で取得
    let getErrorMessage (error: IError) (language: Language) =
        match toResourceKey error with
        | Some resourceKey -> getText language resourceKey
        | None -> error.UserMessage

    /// UIイベントハンドラ用のエラー処理
    let handleError (error: IError) (language: Language) (dispatch: Types.Msg -> unit) =
        // まずドメインエラーに変換する
        let domainError = ErrorTranslationService.translateToDomainError error
        let message = getErrorMessage domainError language

        // エラーカテゴリに基づいて処理を分岐
        match domainError.Category with
        | "Domain" ->
            // ドメイン層のエラーはユーザーに表示
            dispatch (Types.ShowError message)
        | "Infrastructure" ->
            // インフラエラーの場合はログに記録し、ユーザーフレンドリーなメッセージを表示
#if DEBUG
            Browser.Dom.console.error (sprintf "システムエラー: %s" domainError.UserMessage)
#endif
            dispatch (Types.ShowError "システムエラーが発生しました。管理者に連絡してください。")
        | _ ->
            // その他のエラー
            dispatch (Types.ShowError message)
