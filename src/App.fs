module App

open Elmish
open Elmish.React
open UI.State.Types
open UI.State
open UI.Routing.AppRouter

// Program entry point
let main =
    Program.mkProgram AppState.init AppState.update AppRouter
    |> Program.withReactSynchronous "elmish-app"
#if DEBUG
    |> Program.withConsoleTrace
#endif
    |> Program.run
