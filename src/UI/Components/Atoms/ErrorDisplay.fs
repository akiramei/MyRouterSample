namespace UI.Components.Atoms

open Feliz
open Feliz.DaisyUI
open UI.State.ViewModels // ViewModelsを使用
open UI.State.Messages // Messagesを使用
open Domain.Errors
open Domain.ValueObjects.Localization
open Application.Services.ErrorMessageService

/// エラー表示コンポーネント
module ErrorDisplay =

    /// グローバルエラーメッセージの表示
    [<ReactComponent>]
    let GlobalErrorDisplay (model: ErrorDisplayState) (dispatch: AppMsg -> unit) =
        if model.IsVisible then
            Daisy.alert
                [ alert.error
                  prop.className "mb-4"
                  prop.children
                      [ Html.div
                            [ prop.className "flex items-center justify-between w-full"
                              prop.children
                                  [ Html.span [ prop.text (model.Message |> Option.defaultValue "エラーが発生しました") ]
                                    Daisy.button.button
                                        [ button.circle
                                          button.xs
                                          prop.onClick (fun _ -> dispatch ClearError)
                                          prop.children [ Html.span [ prop.text "✕" ] ] ] ] ] ] ]
        else
            Html.none


    /// エラーオブジェクトからメッセージを表示
    [<ReactComponent>]
    let ErrorView (error: IError option) (language: Language) =
        match error with
        | Some err ->
            // ユーザーの言語設定に基づいてエラーメッセージを生成
            let errorMsg = getUserMessage err language
            let errorCode = err.Code

            Daisy.alert
                [ alert.error
                  prop.className "mt-4"
                  prop.children
                      [ Html.div
                            [ prop.className "flex justify-between items-center"
                              prop.children
                                  [ Html.span [ prop.className "flex-grow"; prop.text errorMsg ]
                                    Html.span
                                        [ prop.className "text-xs opacity-70 ml-2"
                                          prop.text (sprintf "エラーコード: %s" errorCode) ] ] ] ] ]
        | None -> Html.none

    /// フィールドレベルのエラー表示
    [<ReactComponent>]
    let FieldError (fieldName: string) (errors: IError list) (language: Language) =
        // 指定されたフィールドに関連するエラーのみをフィルタリング
        let fieldErrors =
            errors
            |> List.filter (fun err ->
                match err with
                | :? DomainError as domainErr ->
                    match domainErr.Details with
                    | ValidationError(field, _, _) when field = fieldName -> true
                    | _ -> false
                | _ -> false)

        match fieldErrors with
        | [] -> Html.none
        | error :: _ ->
            // エラーがある場合は最初のエラーメッセージを表示
            let errorMsg = getUserMessage error language

            Html.div
                [ prop.className "text-error text-sm mt-1"
                  prop.children [ Html.text errorMsg ] ]
