module App

open Elmish
open Elmish.React
open Feliz
open Feliz.Router
open Browser.Dom

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

type Msg =
    | CounterMsg of CounterMsg
    | UserProfileMsg of UserProfileMsg
    | UrlChanged of string list

and CounterMsg =
    | Increment
    | Decrement

and UserProfileMsg =
    | LoadUserData of string
    | UserDataLoaded

let parseUrl (urlSegments: string list) =
    match urlSegments with
    | [] -> Home
    | [ "counter" ] -> Counter
    | [ "user"; username ] -> UserProfile username
    | _ -> NotFound

let init () = 
    let initialUrl = Router.currentPath()
    {
        Counter = { Count = 0 }
        UserProfile = { Username = ""; IsLoading = false }
        CurrentUrl = initialUrl
        CurrentPage = parseUrl initialUrl
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
        { model with CurrentUrl = newUrl; CurrentPage = parseUrl newUrl }, Cmd.none

[<ReactComponent>]
let RouterView (model: Model) (dispatch: Msg -> unit) =
    let isActive path =
        match model.CurrentUrl with
        | [] when path = "" -> "active"
        | [ p ] when p = path -> "active"
        | [ "user"; username ] when path = "user/john" && username = "john" -> "active"
        | [ "user"; username ] when path = "user/alice" && username = "alice" -> "active"
        | _ -> ""

    Html.div [
        prop.className "app-container"
        prop.children [
            Html.nav [
                prop.className "navbar"
                prop.children [
                    Html.ul [
                        prop.className "nav-list"
                        prop.children [
                            Html.li [
                                prop.className ("nav-item " + isActive "")
                                prop.onClick (fun e -> 
                                    e.preventDefault()
                                    Router.navigate("#/counter")
                                    Router.navigate("")
                                )
                                prop.children [
                                    Html.a [
                                        prop.href "/"
                                        prop.text "Home"
                                    ]
                                ]
                            ]
                            Html.li [
                                prop.className ("nav-item " + isActive "counter")
                                prop.onClick (fun e -> 
                                    e.preventDefault()
                                    Router.navigate("#/counter")
                                )
                                prop.children [
                                    Html.a [
                                        prop.href "/counter"
                                        prop.onClick (fun e -> 
                                            e.preventDefault()
                                            Router.navigate("#/counter")
                                        )
                                        prop.text "Counter"
                                    ]
                                ]
                            ]
                            Html.li [
                                prop.className ("nav-item " + isActive "user/john")
                                prop.onClick (fun e -> 
                                    e.preventDefault()
                                    Router.navigate("#/user/john")
                                )
                                prop.children [
                                    Html.a [
                                        prop.href "/user/john"
                                        prop.text "John's Profile"
                                    ]
                                ]
                            ]
                            Html.li [
                                prop.className ("nav-item " + isActive "user/alice")
                                prop.onClick (fun e -> 
                                    e.preventDefault()
                                    Router.navigate("#/user/alice")
                                )
                                prop.children [
                                    Html.a [
                                        prop.href "/user/alice"
                                        prop.text "Alice's Profile"
                                    ]
                                ]
                            ]
                        ]
                    ]
                ]
            ]

            React.router [
                router.hashMode
                router.onUrlChanged (fun urlSegments -> dispatch (UrlChanged urlSegments))
                router.children [
                    match model.CurrentPage with
                    | Home -> 
                        Html.div [
                            prop.className "page"
                            prop.children [
                                Html.h1 "Home"
                                Html.p "Welcome to the Elmish Router Sample App"
                            ]
                        ]
                    | Counter -> 
                        Html.div [
                            prop.className "page"
                            prop.children [
                                Html.h1 "Counter"
                                Html.p (sprintf "Current count: %d" model.Counter.Count)
                                Html.button [
                                    prop.onClick (fun _ -> dispatch (CounterMsg Increment))
                                    prop.text "+"
                                ]
                                Html.button [
                                    prop.onClick (fun _ -> dispatch (CounterMsg Decrement))
                                    prop.text "-"
                                ]
                            ]
                        ]
                    | UserProfile username -> 
                        if model.UserProfile.Username <> username then
                            dispatch (UserProfileMsg (LoadUserData username))

                        Html.div [
                            prop.className "page"
                            prop.children [
                                Html.h1 (sprintf "%s's Profile" username)
                                if model.UserProfile.IsLoading then
                                    Html.p "Loading..."
                                else
                                    Html.p (sprintf "Username: %s" username)
                            ]
                        ]
                    | NotFound -> 
                        Html.div [
                            prop.className "page"
                            prop.children [
                                Html.h1 "Not Found"
                            ]
                        ]
                ]
            ]

            Html.style """
                .app-container {
                    font-family: Arial, sans-serif; 
                    max-width: 800px; 
                    margin: 0 auto; 
                    padding: 20px;
                }

                .navbar {
                    display: flex;
                    background-color: #f0f0f0;
                    padding: 10px;
                    margin-bottom: 20px;
                    border-radius: 5px;
                    border-bottom: 2px solid #ddd;
                }

                .nav-list {
                    display: flex;
                    list-style: none;
                    padding: 0;
                    margin: 0;
                    width: 100%;
                }
                .nav-item {
                    flex-grow: 1;
                    text-align: center;
                    padding: 10px 20px;
                    cursor: pointer; /* ✅ 全体をクリック可能に */
                    transition: background-color 0.3s ease;
                    border-bottom: 3px solid transparent;
                }

                .nav-item a {
                    display: block; /* ✅ <li> 全体がクリック可能な見た目と一致する */
                    width: 100%;
                    height: 100%;
                    text-decoration: none;
                    color: #333;
                    font-weight: bold;
                    padding: 10px 0;
                }

                .nav-item:hover {
                    background-color: #ddd;
                }

                .nav-item.active {
                    background-color: #fff;
                    border-bottom: 3px solid #007bff;
                    font-weight: bold;
                    color: #007bff;
                }

                .content {
                    padding: 20px;
                    background-color: #f9f9f9;
                    border-radius: 5px;
                }

                .page {
                    min-height: 200px;
                }

                button {
                    margin: 0 5px;
                    padding: 5px 10px;
                }
            """
        ]
    ]

let view model dispatch =
    RouterView model dispatch

Program.mkProgram init update view
|> Program.withReactSynchronous "elmish-app"
|> Program.withConsoleTrace
|> Program.run