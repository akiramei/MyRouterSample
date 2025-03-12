namespace UI.State

open Elmish
open UI.Services.RouteService
open UI.State
open UI.State.ViewModels
open UI.State.Messages
open Domain.ValueObjects.User

/// アプリケーション状態管理
module AppState =
    // 初期化関数
    let initHomePage () = { IsLoading = false }

    let initCounterPage () =
        { Count = 0
          IsIncrementing = false
          IsDecrementing = false }

    let initUserProfilePage () =
        { UserId = None
          Username = ""
          IsLoading = false
          IsEditing = false
          ValidationErrors = [] }

    let initLoginPage () =
        { Username = ""
          Password = ""
          Language = English
          IsSubmitting = false
          Error = None }

    let initErrorDisplay () = { IsVisible = false; Message = None }

    // 開発環境用のダミーユーザープロファイル
    let createDevUserProfile () =
        { UserId = "dev-user-001"
          Username = "開発者ユーザー"
          Language = Japanese
          IsAuthenticated = true
          LastLoginAt = Some System.DateTime.Now }

    /// アプリケーション全体の状態を初期化
    let init () =
        let initialUrl = currentPath ()

        // 開発環境の自動ログイン設定
#if DEBUG
        // true に設定すると開発環境で自動ログインする
        let useAutoLoginInDev = true

        // 開発環境の初期状態設定
        let devUserOpt =
            if useAutoLoginInDev then
                Some(createDevUserProfile ())
            else
                None

        let initialPage =
            match initialUrl with
            | [] when useAutoLoginInDev -> Home // 空URLの場合は自動的にホーム画面へ
            | segments -> parseUrl segments // 特定のURLが指定されている場合はそのページへ
#else
        // 本番環境の初期状態設定
        let devUserOpt = None
        let initialPage = parseUrl initialUrl
#endif

        let initialState =
            { CurrentUser = devUserOpt
              CurrentUrl = initialUrl
              CurrentPage = initialPage
              HomePage = initHomePage ()
              CounterPage = initCounterPage ()
              UserProfilePage = initUserProfilePage ()
              LoginPage = initLoginPage ()
              ErrorDisplay = initErrorDisplay () }

        // 開発環境でホーム画面に自動遷移するコマンド
#if DEBUG
        let initialCmd =
            if useAutoLoginInDev && (initialUrl = [] || initialUrl = [ "" ]) then
                // 初期化時に一度だけナビゲート
                navigateCmd "home"
            else
                Cmd.none
#else
        let initialCmd = Cmd.none
#endif

        initialState, initialCmd
