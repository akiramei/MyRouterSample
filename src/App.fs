module App

open Elmish
open Elmish.React
open UI.State.Messages
open UI.State
open UI.Routing.AppRouter
open Shared.I18n.TranslationService
open Common.Platform

// 翻訳ファイルを読み込む
loadTranslationsFromFile () |> ignore

// Program entry point
let main =
    Program.mkProgram AppState.init AppStateUpdate.update AppRouter
    |> Program.withReactSynchronous "elmish-app"
    // 開発環境の場合のみコンソールトレースを有効化
    |> (if PlatformServices.Environment.isDevelopment then 
           Program.withConsoleTrace 
        else 
           id)
    |> Program.run