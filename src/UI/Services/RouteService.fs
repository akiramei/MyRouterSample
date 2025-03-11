namespace UI.Services

open Domain.ValueObjects.Types
open Elmish
open Feliz
open Feliz.Router
open UI.State.Messages

/// URLとアプリケーション状態間のマッピングを担当するサービス
module RouteService =
    /// URLセグメントをアプリケーションページに変換
    let parseUrl (urlSegments: string list) =
        match urlSegments with
        | [] -> Login
        | [ "" ] -> Home // 空文字列でもホームに遷移
        | [ "home" ] -> Home
        | [ "counter" ] -> Counter
        | [ "user"; username ] -> UserProfile username
        | _ -> NotFound

    /// ページからURLセグメントへの変換（オプション）
    let formatUrl (page: Page) =
        match page with
        | Login -> [ "" ]
        | Home -> [ "home" ]
        | Counter -> [ "counter" ]
        | UserProfile username -> [ "user"; username ]
        | NotFound -> [ "not-found" ]

    /// 現在のパスを取得
    let currentPath = Feliz.Router.Router.currentPath

    /// 指定されたパスに遷移するコマンドを生成
    let navigateCmd (path: string) : Cmd<AppMsg> =
        Feliz.Router.Router.navigate (path)
        Cmd.ofMsg (UrlChanged [ path ])

    /// ページに基づいて遷移コマンドを生成（オプション）
    let navigateToPage (page: Page) : Cmd<AppMsg> =
        let path =
            match formatUrl page with
            | [] -> ""
            | segments -> String.concat "/" segments

        navigateCmd path

    /// 指定されたパスが現在のURLとマッチするかを判定し、
    /// アクティブなナビゲーションアイテムのCSSクラスを返す
    let getActiveClass (currentUrl: string list) (path: string) =
        match currentUrl, path with
        | [], "" -> "active"
        | [ "" ], "home" -> "active" // 空文字列はホームと同等
        | [ p ], path when p = path -> "active"
        | [ "user"; username ], "user/john" when username = "john" -> "active"
        | [ "user"; username ], "user/alice" when username = "alice" -> "active"
        | _ -> ""
