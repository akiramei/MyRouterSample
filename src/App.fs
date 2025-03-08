module App

open Elmish
open Elmish.React
open Feliz
open Feliz.DaisyUI
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
        
        Daisy.navbar [
            prop.className "mb-4 shadow-lg bg-base-200 rounded-box"
            prop.children [
                Html.div [
                    prop.className "flex-1 px-2"
                    prop.children [
                        Daisy.menu [
                            prop.className "menu-horizontal px-1"
                            prop.children [
                                NavItem "" "Home" (isActive "")
                                NavItem "counter" "Counter" (isActive "counter")
                                NavItem "user/john" "John's Profile" (isActive "user/john")
                                NavItem "user/alice" "Alice's Profile" (isActive "user/alice")
                            ]
                        ]
                    ]
                ]
            ]
        ]

    [<ReactComponent>]
    let HomePage () =
        Daisy.hero [
            prop.className "bg-primary rounded-box"
            prop.children [
                Daisy.heroContent [
                    prop.className "text-center"
                    prop.children [
                        Html.h1 [
                            prop.className "text-4xl font-bold text-primary-content" // テキスト色を背景色に合わせたコントラストの高い色に変更
                            prop.text "Home"
                        ]
                        Html.p [
                            prop.className "py-6 text-primary-content" // テキスト色を背景色に合わせたコントラストの高い色に変更
                            prop.text "Welcome to the Elmish Router Sample App"
                        ]
                    ]
                ]
            ]
        ]
    
    [<ReactComponent>]
    let CounterPage (model: CounterModel) (dispatch: CounterMsg -> unit) =
        Daisy.card [
            prop.className "shadow-lg bg-base-100 border"
            prop.children [
                Daisy.cardBody [
                    Html.h2 [
                        prop.className "card-title"
                        prop.text "Counter"
                    ]
                    Html.p [
                        prop.className "py-4"
                        prop.text (sprintf "Current count: %d" model.Count)
                    ]
                    Daisy.cardActions [
                        Html.div [
                            Daisy.join [
                                Daisy.button.button [
                                    prop.className "btn-primary btn-outline"
                                    join.item
                                    prop.onClick (fun _ -> dispatch Increment)
                                    prop.text "+"
                                ]
                                Daisy.button.button [
                                    prop.className "btn-secondary btn-outline"
                                    join.item
                                    prop.onClick (fun _ -> dispatch Decrement)
                                    prop.text "-"
                                ]
                            ]
                        ]
                    ]
                ]
            ]
        ]
    
    [<ReactComponent>]
    let UserProfilePage (username: string) (isLoading: bool) =
        Daisy.card [
            prop.className "shadow-lg bg-base-100 border"
            prop.children [
                Daisy.cardBody [
                    Html.h2 [
                        prop.className "card-title"
                        prop.text (sprintf "%s's Profile" username)
                    ]
                    
                    if isLoading then
                        Html.div [
                            prop.className "flex justify-center py-4"
                            prop.children [
                                Daisy.loading [
                                    prop.className "loading-spinner loading-lg"
                                ]
                            ]
                        ]
                    else
                        Html.p [
                            prop.className "py-4"
                            prop.text (sprintf "Username: %s" username)
                        ]
                ]
            ]
        ]
    
    [<ReactComponent>]
    let NotFoundPage () =
        Daisy.card [
            prop.className "shadow-lg bg-base-100 border image-full"
            prop.children [
                Daisy.cardBody [
                    Html.h2 [
                        prop.className "card-title text-error"
                        prop.text "Not Found"
                    ]
                    Html.p [
                        prop.text "The page you're looking for doesn't exist."
                    ]
                    Daisy.button.a [
                        prop.className "btn-primary"
                        prop.href "#/"
                        prop.text "Go Home"
                    ]
                ]
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

// Main view
let view (model: Types.Model) (dispatch: Types.Msg -> unit) =
    Html.div [
        prop.className "min-h-screen bg-base-300"
        prop.children [
            Html.div [
                prop.className "container mx-auto px-4 py-6"
                prop.children [
                    Components.Navigation model.CurrentUrl
                    Components.MainContent model dispatch
                ]
            ]
        ]
    ]

// Program
let main =
    Program.mkProgram State.init State.update view
    |> Program.withReactSynchronous "elmish-app"
    |> Program.withConsoleTrace
    |> Program.run