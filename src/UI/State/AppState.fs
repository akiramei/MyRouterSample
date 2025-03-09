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

    let init () = 
        let initialUrl = currentPath()
        {
            Counter = initCounter()
            UserProfile = initUserProfile()
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

    let update msg model =
        match msg with
        | CounterMsg counterMsg ->
            let counter, cmd = updateCounter counterMsg model.Counter
            { model with Counter = counter }, Cmd.map CounterMsg cmd
        | UserProfileMsg profileMsg ->
            let profile, cmd = updateUserProfile profileMsg model.UserProfile
            { model with UserProfile = profile }, Cmd.map UserProfileMsg cmd
        | UrlChanged newUrl ->
            { model with CurrentUrl = newUrl; CurrentPage = parseUrl newUrl }, Cmd.none