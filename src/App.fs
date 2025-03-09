module App

open Elmish
open Elmish.React
open UI.State.Types
open UI.State
open UI.Components.Templates

// Program entry point
let main =
    Program.mkProgram AppState.init AppState.update MainLayout.MainLayout
    |> Program.withReactSynchronous "elmish-app"
    #if DEBUG
    |> Program.withConsoleTrace
    #endif
    |> Program.run