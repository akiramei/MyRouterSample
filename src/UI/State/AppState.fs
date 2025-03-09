namespace UI.State

open Elmish
open Domain.ValueObjects.Types
open UI.Services.Router
open UI.State.Types

/// Main application state management
module AppState =
    // Initialize state
    let initCounter () = { Count = 0 }

    let initUserProfile () = { Username = ""; IsLoading = false }
    
    let initLogin () = { 
        Username = ""; 
        Password = ""; 
        Language = Domain.ValueObjects.User.English; 
        ErrorMessage = None 
    }

    let init () = 
        let initialUrl = currentPath()
        {
            Counter = initCounter()
            UserProfile = initUserProfile()
            Login = initLogin()
            CurrentUser = None
            CurrentUrl = initialUrl
            CurrentPage = parseUrl initialUrl
        }, Cmd.none

    // Update functions
    let updateCounter msg model =
        match msg with
        | Increment -> { model with Count = model.Count + 1 }, Cmd.none
        | Decrement -> { model with Count = model.Count - 1 }, Cmd.none

    let updateUserProfile msg model =
        match msg with
        | LoadUserData username ->
            { model with IsLoading = true; Username = username }, 
            Cmd.ofMsg (UserDataLoaded)
        | UserDataLoaded ->
            { model with IsLoading = false }, Cmd.none
            
    let updateLogin msg (model: LoginModel) =
        match msg with
        | SetUsername username ->
            { model with Username = username }, Cmd.none
        | SetPassword password ->
            { model with Password = password }, Cmd.none
        | SetLanguage language ->
            { model with Language = language }, Cmd.none
        | LoginSubmit ->
            // バリデーション確認
            if System.String.IsNullOrWhiteSpace model.Username then
                { model with ErrorMessage = Some Shared.I18n.Resources.UsernameRequired }, Cmd.none
            elif System.String.IsNullOrWhiteSpace model.Password then
                { model with ErrorMessage = Some Shared.I18n.Resources.PasswordRequired }, Cmd.none
            else
                let userProfile : Domain.ValueObjects.User.UserProfile = {
                    Username = model.Username
                    Language = model.Language
                    IsAuthenticated = true
                }
                model, Cmd.ofMsg (LoginSuccess userProfile)
        | LoginSuccess _ ->
            { model with ErrorMessage = None }, Cmd.none
        | LoginFailed _ ->
            { model with ErrorMessage = Some Shared.I18n.Resources.ValidationError }, Cmd.none

    let update msg model =
        match msg with
        | LoginMsg loginMsg ->
            match loginMsg with
            | LoginSuccess userProfile ->
                let newLoginModel, loginCmd = updateLogin loginMsg model.Login
                let cmd = Cmd.batch [
                    Cmd.map LoginMsg loginCmd
                    navigateCmd "home"
                ]
                { model with 
                    Login = newLoginModel
                    CurrentUser = Some userProfile 
                    CurrentPage = Home }, cmd
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
                { model with UserProfile = profile }, Cmd.map UserProfileMsg cmd
        | UrlChanged newUrl ->
            let newPage = parseUrl newUrl
            
            // ログインしていない場合は、ログイン画面のみアクセス可能
            match newPage, model.CurrentUser with
            | Login, _ ->
                { model with CurrentUrl = newUrl; CurrentPage = newPage }, Cmd.none
            | _, None ->
                { model with CurrentUrl = []; CurrentPage = Login }, 
                navigateCmd ""
            | _, Some _ ->
                { model with CurrentUrl = newUrl; CurrentPage = newPage }, Cmd.none
        | Logout ->
            { model with 
                CurrentUser = None
                CurrentPage = Login
                Login = initLogin() }, 
            navigateCmd ""