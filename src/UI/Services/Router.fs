namespace UI.Services

open Domain.ValueObjects.Types
open Feliz
open Feliz.Router

/// Router functionality for application navigation
module Router =
    let parseUrl (urlSegments: string list) =
        match urlSegments with
        | [] -> Home
        | [ "counter" ] -> Counter
        | [ "user"; username ] -> UserProfile username
        | _ -> NotFound

    let getActiveClass (currentUrl: string list) (path: string) =
        match currentUrl, path with
        | [], "" -> "active"
        | [ p ], path when p = path -> "active"
        | [ "user"; username ], "user/john" when username = "john" -> "active"
        | [ "user"; username ], "user/alice" when username = "alice" -> "active"
        | _ -> ""

    // Get current path from router
    let currentPath = Router.currentPath