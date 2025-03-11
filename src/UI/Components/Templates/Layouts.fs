namespace UI.Components.Templates

open Feliz
open UI.State.ViewModels
open UI.State.Messages
open UI.Components.Molecules.Navigation
open UI.Components.Atoms.ErrorDisplay

/// レイアウトコンポーネント
module Layouts =
    [<ReactComponent>]
    let AuthenticatedLayout (state: ApplicationState) (dispatch: AppMsg -> unit) (content: ReactElement) =
        Html.div
            [ prop.className "min-h-screen bg-base-300"
              prop.children
                  [ Html.div
                        [ prop.className "container mx-auto px-4 py-6"
                          prop.children
                              [ Navigation state.CurrentUrl state.CurrentUser dispatch
                                // グローバルエラー表示の追加
                                GlobalErrorDisplay state.ErrorDisplay (ClearError >> dispatch)
                                content ] ] ] ]

    [<ReactComponent>]
    let UnauthenticatedLayout (state: ApplicationState) (dispatch: AppMsg -> unit) (content: ReactElement) =
        Html.div
            [ prop.className "min-h-screen bg-base-300"
              prop.children
                  [ Html.div
                        [ prop.className "container mx-auto px-4 py-6 max-w-md"
                          prop.children
                              [
                                // グローバルエラー表示の追加
                                GlobalErrorDisplay state.ErrorDisplay (ClearError >> dispatch)
                                content ] ] ] ]
