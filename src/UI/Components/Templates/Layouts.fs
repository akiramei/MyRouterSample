namespace UI.Components.Templates

open Feliz
open UI.State.Types
open UI.Components.Molecules.Navigation
open UI.Components.Atoms.ErrorDisplay

/// レイアウトコンポーネント
module Layouts =
    [<ReactComponent>]
    let AuthenticatedLayout (model: Model) (dispatch: Msg -> unit) (content: ReactElement) =
        Html.div [
            prop.className "min-h-screen bg-base-300"
            prop.children [
                Html.div [
                    prop.className "container mx-auto px-4 py-6"
                    prop.children [
                        Navigation model.CurrentUrl model.CurrentUser dispatch
                        // グローバルエラー表示の追加
                        ErrorDisplay model.ErrorDisplay dispatch
                        content
                    ]
                ]
            ]
        ]
        
    [<ReactComponent>]
    let UnauthenticatedLayout (model: Model) (dispatch: Msg -> unit) (content: ReactElement) =
        Html.div [
            prop.className "min-h-screen bg-base-300"
            prop.children [
                Html.div [
                    prop.className "container mx-auto px-4 py-6 max-w-md"
                    prop.children [
                        // グローバルエラー表示の追加
                        ErrorDisplay model.ErrorDisplay dispatch
                        content
                    ]
                ]
            ]
        ]