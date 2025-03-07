module App

open Elmish
open Elmish.React
open Feliz
open Feliz.Router

// Types
module Types =
    type Page =
        | Home
        | Counter
        | UserProfile of string
        | NotFound

    type CounterModel = { Count: int }
    type UserProfileModel = { Username: string; IsLoading: bool }

    type Model = {
        Counter: CounterModel
        UserProfile: UserProfileModel
        CurrentUrl: string list
        CurrentPage: Page
    }

    type CounterMsg =
        | Increment
        | Decrement

    type UserProfileMsg =
        | LoadUserData of string
        | UserDataLoaded

    type Msg =
        | CounterMsg of CounterMsg
        | UserProfileMsg of UserProfileMsg
        | UrlChanged of string list

// Router functions
module Router =
    open Types
    
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

// Init and Update functions
module State =
    open Types
    
    let initCounter () = { Count = 0 }
    
    let initUserProfile () = { Username = ""; IsLoading = false }
    
    let init () = 
        let initialUrl = Router.currentPath()
        {
            Counter = initCounter()
            UserProfile = initUserProfile()
            CurrentUrl = initialUrl
            CurrentPage = Router.parseUrl initialUrl
        }, Cmd.none
    
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
            { model with CurrentUrl = newUrl; CurrentPage = Router.parseUrl newUrl }, Cmd.none

// Components
module Components =
    open Types
    
    [<ReactComponent>]
    let NavItem (path: string) (text: string) (isActive: string) =
        let hashPath = if path = "" then "" else "#/" + path
        
        Html.li [
            prop.className ("nav-item " + isActive)
            prop.onClick (fun e -> 
                e.preventDefault()
                Router.navigate hashPath
            )
            prop.children [
                Html.a [
                    prop.href ("/" + path)
                    prop.text text
                ]
            ]
        ]
    
    [<ReactComponent>]
    let Navigation (currentUrl: string list) =
        let isActive = Router.getActiveClass currentUrl
        
        Html.nav [
            prop.className "navbar"
            prop.children [
                Html.ul [
                    prop.className "nav-list"
                    prop.children [
                        NavItem "" "Home" (isActive "")
                        NavItem "counter" "Counter" (isActive "counter")
                        NavItem "user/john" "John's Profile" (isActive "user/john")
                        NavItem "user/alice" "Alice's Profile" (isActive "user/alice")
                    ]
                ]
            ]
        ]
    
    [<ReactComponent>]
    let HomePage () =
        Html.div [
            prop.className "page"
            prop.children [
                Html.h1 "Home"
                Html.p "Welcome to the Elmish Router Sample App"
            ]
        ]
    
    [<ReactComponent>]
    let CounterPage (model: CounterModel) (dispatch: CounterMsg -> unit) =
        Html.div [
            prop.className "page"
            prop.children [
                Html.h1 "Counter"
                Html.p (sprintf "Current count: %d" model.Count)
                Html.button [
                    prop.onClick (fun _ -> dispatch Increment)
                    prop.text "+"
                ]
                Html.button [
                    prop.onClick (fun _ -> dispatch Decrement)
                    prop.text "-"
                ]
            ]
        ]
    
    [<ReactComponent>]
    let UserProfilePage (username: string) (isLoading: bool) =
        Html.div [
            prop.className "page"
            prop.children [
                Html.h1 (sprintf "%s's Profile" username)
                if isLoading then
                    Html.p "Loading..."
                else
                    Html.p (sprintf "Username: %s" username)
            ]
        ]
    
    [<ReactComponent>]
    let NotFoundPage () =
        Html.div [
            prop.className "page"
            prop.children [
                Html.h1 "Not Found"
            ]
        ]
    
    [<ReactComponent>]
    let MainContent (model: Model) (dispatch: Msg -> unit) =
        let checkAndLoadProfile username =
            if model.UserProfile.Username <> username then
                dispatch (UserProfileMsg (LoadUserData username))
        
        React.router [
            router.hashMode
            router.onUrlChanged (fun urlSegments -> dispatch (UrlChanged urlSegments))
            router.children [
                match model.CurrentPage with
                | Types.Home -> HomePage()
                | Types.Counter -> 
                    CounterPage 
                        model.Counter 
                        (fun msg -> dispatch (CounterMsg msg))
                | Types.UserProfile username -> 
                    checkAndLoadProfile username
                    UserProfilePage 
                        username 
                        model.UserProfile.IsLoading
                | Types.NotFound -> NotFoundPage()
            ]
        ]

// スタイルモジュールは削除（外部CSSファイルを使用するため）

// Main view
let view (model: Types.Model) (dispatch: Types.Msg -> unit) =
    Html.div [
        prop.className "app-container"
        prop.children [
            Components.Navigation model.CurrentUrl
            Components.MainContent model dispatch
        ]
    ]

// Program
let main =
    Program.mkProgram State.init State.update view
    |> Program.withReactSynchronous "elmish-app"
    |> Program.withConsoleTrace
    |> Program.run