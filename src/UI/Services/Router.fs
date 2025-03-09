namespace UI.Services

open Domain.ValueObjects.Types
open Elmish
open Feliz
open Feliz.Router
open UI.State.Types

/// Router functionality for application navigation
module Router =
    let parseUrl (urlSegments: string list) =
        match urlSegments with
        | [] -> Login
        | [ "" ] -> Home  // 空文字列でもホームに遷移
        | [ "home" ] -> Home
        | [ "counter" ] -> Counter
        | [ "user"; username ] -> UserProfile username
        | _ -> NotFound

    let getActiveClass (currentUrl: string list) (path: string) =
        match currentUrl, path with
        | [], "" -> "active"
        | [ "" ], "home" -> "active"  // 空文字列はホームと同等
        | [ p ], path when p = path -> "active"
        | [ "user"; username ], "user/john" when username = "john" -> "active"
        | [ "user"; username ], "user/alice" when username = "alice" -> "active"
        | _ -> ""

    // Get current path from router
    let currentPath = Feliz.Router.Router.currentPath
    
    // ナビゲーション用の補助関数
    let navigateCmd (path: string) : Cmd<Msg> =
        Feliz.Router.Router.navigate(path)
        Cmd.ofMsg (UrlChanged [path])