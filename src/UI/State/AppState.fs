namespace UI.State

open Elmish
open Domain.ValueObjects.Types
open UI.Services.RouteService
open UI.State.Types
open Domain.Errors

/// Main application state management
module AppState =
    // Initialize state
    let initCounter () = { Count = 0 }

    let initUserProfile () = { Username = ""; IsLoading = false }

    let initErrorDisplay () = { IsVisible = false; Message = None }

    let initLogin () =
        { Username = ""
          Password = ""
          Language = Domain.ValueObjects.User.English
          ErrorMessage = None
          Error = None }

    let init () =
        let initialUrl = currentPath ()

        { Counter = initCounter ()
          UserProfile = initUserProfile ()
          Login = initLogin ()
          CurrentUser = None
          CurrentUrl = initialUrl
          CurrentPage = parseUrl initialUrl
          ErrorDisplay = initErrorDisplay () },
        Cmd.none

    // Update functions
    let updateCounter msg model =
        match msg with
        | Increment -> { model with Count = model.Count + 1 }, Cmd.none
        | Decrement -> { model with Count = model.Count - 1 }, Cmd.none

    let updateUserProfile msg model =
        match msg with
        | LoadUserData username ->
            { model with
                IsLoading = true
                Username = username },
            Cmd.ofMsg (UserDataLoaded)
        | UserDataLoaded -> { model with IsLoading = false }, Cmd.none
        | UserDataError error ->
            // エラー処理の追加 - UserProfileMsg型のメッセージを返す
            { model with IsLoading = false }, 
            Cmd.ofMsg (ShowUserProfileError (ErrorHelpers.toUserMessage error))
        | ShowUserProfileError _ ->
            // プロファイル固有のエラー - この処理は上位のupdateで行う
            model, Cmd.none

    let validateLogin (model: LoginModel) =
        // Railway Oriented Programming パターンによるバリデーション
        let validateUsername (model: LoginModel) =
            if System.String.IsNullOrWhiteSpace model.Username then
                let error = ErrorHelpers.asDomainError (ValidationError("username", "ユーザー名は必須です")) None
                Failure error
            else
                Success model
                
        let validatePassword (model: LoginModel) =
            if System.String.IsNullOrWhiteSpace model.Password then
                let error = ErrorHelpers.asDomainError (ValidationError("password", "パスワードは必須です")) None
                Failure error
            else
                Success model
                
        validateUsername model
        |> Railway.bind validatePassword

    let updateLogin msg (model: LoginModel) =
        match msg with
        | SetUsername username -> { model with Username = username }, Cmd.none
        | SetPassword password -> { model with Password = password }, Cmd.none
        | SetLanguage language -> { model with Language = language }, Cmd.none
        | LoginSubmit ->
            // Railway Oriented Programming パターンでのバリデーション
            match validateLogin model with
            | Success validModel ->
                let userProfile: Domain.ValueObjects.User.UserProfile =
                    { Username = validModel.Username
                      Language = validModel.Language
                      IsAuthenticated = true }

                { model with 
                    Error = None
                    ErrorMessage = None }, 
                Cmd.ofMsg (LoginSuccess userProfile)
            | Failure error ->
                // エラー処理
                { model with 
                    Error = Some error
                    ErrorMessage = UI.State.ErrorHandling.toResourceKey error }, 
                Cmd.ofMsg (LoginFailed error)
        | LoginSuccess _ -> 
            { model with 
                Error = None
                ErrorMessage = None }, 
            Cmd.none
        | LoginFailed error ->
            { model with
                Error = Some error
                ErrorMessage = UI.State.ErrorHandling.toResourceKey error },
            Cmd.none

    let update msg model =
        match msg with
        | LoginMsg loginMsg ->
            match loginMsg with
            | LoginSuccess userProfile ->
                let newLoginModel, loginCmd = updateLogin loginMsg model.Login
                let cmd = Cmd.batch [ Cmd.map LoginMsg loginCmd; navigateCmd "home" ]

                { model with
                    Login = newLoginModel
                    CurrentUser = Some userProfile
                    CurrentPage = Home },
                cmd
            | LoginFailed error ->
                let login, cmd = updateLogin loginMsg model.Login
                let errorMessage = 
                    match model.CurrentUser with
                    | Some user -> UI.State.ErrorHandling.getErrorMessage error user.Language
                    | None -> ErrorHelpers.toUserMessage error
                
                { model with 
                    Login = login
                    ErrorDisplay = { IsVisible = true; Message = Some errorMessage } }, 
                Cmd.batch [ Cmd.map LoginMsg cmd ]
            | _ ->
                let login, cmd = updateLogin loginMsg model.Login
                { model with Login = login }, Cmd.map LoginMsg cmd
        | CounterMsg counterMsg ->
            // ログインチェック
            match model.CurrentUser with
            | None -> model, navigateCmd ""
            | Some _ ->
                let counter, cmd = updateCounter counterMsg model.Counter
                { model with Counter = counter }, Cmd.map CounterMsg cmd
        | UserProfileMsg profileMsg ->
            // ログインチェック
            match model.CurrentUser with
            | None -> model, navigateCmd ""
            | Some _ ->
                let profile, cmd = updateUserProfile profileMsg model.UserProfile
                
                // ShowUserProfileErrorメッセージの場合、グローバルエラーも設定
                let (newModel, globalCmd) =
                    match profileMsg with
                    | ShowUserProfileError errorMessage ->
                        { model with ErrorDisplay = { IsVisible = true; Message = Some errorMessage } },
                        Cmd.ofMsg (ShowError errorMessage)
                    | _ -> model, Cmd.none
                
                { newModel with UserProfile = profile }, 
                Cmd.batch [ Cmd.map UserProfileMsg cmd; globalCmd ]
        | UrlChanged newUrl ->
            let newPage = parseUrl newUrl

            // ログインしていない場合は、ログイン画面のみアクセス可能
            match newPage, model.CurrentUser with
            | Login, _ ->
                { model with
                    CurrentUrl = newUrl
                    CurrentPage = newPage },
                Cmd.none
            | _, None ->
                { model with
                    CurrentUrl = []
                    CurrentPage = Login },
                navigateCmd ""
            | _, Some _ ->
                { model with
                    CurrentUrl = newUrl
                    CurrentPage = newPage },
                Cmd.none
        | Logout ->
            { model with
                CurrentUser = None
                CurrentPage = Login
                Login = initLogin () },
            navigateCmd ""
        | ShowError message ->
            { model with
                ErrorDisplay = { IsVisible = true; Message = Some message } },
            Cmd.none
        | ClearError ->
            { model with
                ErrorDisplay = { IsVisible = false; Message = None } },
            Cmd.none