module App

open Elmish
open Elmish.React
open UI.State.Types
open UI.State
open UI.Routing.AppRouter
open Shared.I18n.TranslationService

// 翻訳ファイルを読み込む
#if FABLE_COMPILER
loadTranslationsFromFile () |> ignore
#endif

// Program entry point
let main =
    Program.mkProgram AppState.init AppState.update AppRouter
    |> Program.withReactSynchronous "elmish-app"
#if DEBUG
    |> Program.withConsoleTrace
#endif
    |> Program.run
