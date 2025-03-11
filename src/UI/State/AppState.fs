namespace UI.State

open Elmish
open Domain.ValueObjects.Types
open UI.Services.RouteService
open UI.State.ViewModels
open Domain.Errors
open UI.Errors
open Application.ErrorTranslation
open Application.Services.ErrorMessageService
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

    /// アプリケーション全体の状態を初期化
    let init () =
        let initialUrl = currentPath ()

        let initialState =
            { CurrentUser = None
              CurrentUrl = initialUrl
              CurrentPage = parseUrl initialUrl
              HomePage = initHomePage ()
              CounterPage = initCounterPage ()
              UserProfilePage = initUserProfilePage ()
              LoginPage = initLoginPage ()
              ErrorDisplay = initErrorDisplay () }

        initialState, Cmd.none
