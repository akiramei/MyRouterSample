namespace UI.Components.Atoms

open Feliz
open Feliz.DaisyUI
open UI.State.Types

/// エラー表示コンポーネント
module ErrorDisplay =
    [<ReactComponent>]
    let ErrorDisplay (model: ErrorDisplay) (dispatch: Msg -> unit) =
        if model.IsVisible then
            Daisy.alert [
                alert.error
                prop.className "mb-4"
                prop.children [
                    Html.div [
                        prop.className "flex items-center justify-between w-full"
                        prop.children [
                            Html.span [ 
                                prop.text (model.Message |> Option.defaultValue "エラーが発生しました") 
                            ]
                            Daisy.button.button [
                                button.circle
                                button.xs
                                prop.onClick (fun _ -> dispatch ClearError)
                                prop.children [
                                    Html.span [ prop.text "✕" ]
                                ]
                            ]
                        ]
                    ]
                ]
            ]
        else
            Html.none