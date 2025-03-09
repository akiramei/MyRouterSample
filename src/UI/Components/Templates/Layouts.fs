namespace UI.Components.Templates

open Feliz
open UI.State.Types
open UI.Components.Molecules.Navigation

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
                        content
                    ]
                ]
            ]
        ]
        
    [<ReactComponent>]
    let UnauthenticatedLayout (content: ReactElement) =
        Html.div [
            prop.className "min-h-screen bg-base-300"
            prop.children [
                content
            ]
        ]