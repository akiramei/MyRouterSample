namespace UI.State

open Elmish
open UI.Services.RouteService
open UI.State
open UI.State.ViewModels
open Domain.ValueObjects.Localization
open Domain.ValueObjects.User
open Common.Platform

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

        // 環境設定（プラットフォーム抽象化層を使用）
        let isDevelopment = PlatformServices.Environment.isDevelopment
        // 開発環境の自動ログイン設定
        let useAutoLoginInDev = isDevelopment && true // trueに設定すると開発環境で自動ログイン

        // 開発環境と本番環境の初期状態設定
        let devUserOpt =
            if useAutoLoginInDev then
                Some(createDevUserProfile ())
            else
                None

        let initialPage =
            match initialUrl with
            | [] when useAutoLoginInDev -> Home // 空URLの場合は自動的にホーム画面へ
            | segments -> parseUrl segments // 特定のURLが指定されている場合はそのページへ

        let initialState =
            { CurrentUser = devUserOpt
              CurrentUrl = initialUrl
              CurrentPage = initialPage
              HomePage = initHomePage ()
              CounterPage = initCounterPage ()
              UserProfilePage = initUserProfilePage ()
              LoginPage = initLoginPage ()
              ErrorDisplay = initErrorDisplay () }

        // 初期コマンドの設定
        let initialCmd =
            if useAutoLoginInDev && (initialUrl = [] || initialUrl = [ "" ]) then
                // 開発環境では初期化時に一度だけナビゲート
                navigateCmd "home"
            else
                Cmd.none

        initialState, initialCmd