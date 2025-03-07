// App.fs
module App

open Elmish
open Elmish.React
open Feliz
open Feliz.Router
open Browser.Dom

// ページの定義
type Page =
    | Home
    | Counter
    | UserProfile of username: string

// ページごとのモデル
type CounterModel = { Count: int }
type UserProfileModel = { Username: string; IsLoading: bool }

// アプリケーション全体のモデル
type Model =
    { CurrentPage: Page
      CounterModel: CounterModel
      UserProfileModel: UserProfileModel }

// ページごとのメッセージ
type CounterMsg =
    | Increment
    | Decrement

type UserProfileMsg =
    | LoadUserData
    | UserDataLoaded

// アプリケーション全体のメッセージ
type Msg =
    | CounterMsg of CounterMsg
    | UserProfileMsg of UserProfileMsg
    | NavigateTo of Page
    | UrlChanged of string list

// URL解析
let parseUrl (segments: string list) =
    console.log ("URL segments:", segments)

    match segments with
    | [] -> Home
    | [ "counter" ] -> Counter
    | [ "user"; username ] -> UserProfile username
    | _ -> Home

// URL生成
let toUrl =
    function
    | Home -> []
    | Counter -> [ "counter" ]
    | UserProfile username -> [ "user"; username ]

// 初期状態
let init () : Model * Cmd<Msg> =
    console.log ("init関数が呼ばれました")

    let initialModel =
        { CurrentPage = Home
          CounterModel = { Count = 0 }
          UserProfileModel = { Username = ""; IsLoading = false } }

    // 初期URLに基づいたページ設定
    let initialUrl = Router.currentUrl ()
    console.log ("初期URL:", initialUrl)

    let initialPage = parseUrl initialUrl
    console.log ("初期ページ:", initialPage)

    let updatedModel =
        { initialModel with
            CurrentPage = initialPage }

    match initialPage with
    | UserProfile username ->
        // ユーザープロファイルページの場合、データをロード
        let userModel =
            { updatedModel.UserProfileModel with
                Username = username
                IsLoading = true }

        { updatedModel with
            UserProfileModel = userModel },
        Cmd.ofMsg (UserProfileMsg LoadUserData)
    | _ -> updatedModel, Cmd.none

// カウンターページのupdate関数
let updateCounter (msg: CounterMsg) (model: CounterModel) : CounterModel * Cmd<CounterMsg> =
    match msg with
    | Increment -> { model with Count = model.Count + 1 }, Cmd.none
    | Decrement -> { model with Count = model.Count - 1 }, Cmd.none

// ユーザープロファイルページのupdate関数
let updateUserProfile (msg: UserProfileMsg) (model: UserProfileModel) : UserProfileModel * Cmd<UserProfileMsg> =
    match msg with
    | LoadUserData ->
        // 実際のアプリケーションでは、ここでHTTPリクエストなどを行う
        // 簡略化のため、すぐにロード完了とする
        { model with IsLoading = false }, Cmd.ofMsg UserDataLoaded
    | UserDataLoaded -> model, Cmd.none

// メインのupdate関数
let update (msg: Msg) (model: Model) : Model * Cmd<Msg> =
    match msg with
    | CounterMsg subMsg ->
        let counterModel, counterCmd = updateCounter subMsg model.CounterModel

        { model with
            CounterModel = counterModel },
        Cmd.map CounterMsg counterCmd

    | UserProfileMsg subMsg ->
        let userModel, userCmd = updateUserProfile subMsg model.UserProfileModel

        { model with
            UserProfileModel = userModel },
        Cmd.map UserProfileMsg userCmd

    | NavigateTo page ->
        match page with
        | Home ->
            console.log ("ナビゲーション: Home")
            Router.navigate ("")
            model, Cmd.none
        | Counter ->
            console.log ("ナビゲーション: Counter")
            Router.navigate ("counter")
            model, Cmd.none
        | UserProfile username ->
            console.log ("ナビゲーション: User", username)
            Router.navigate (sprintf "user/%s" username)
            model, Cmd.none

    | UrlChanged segments ->
        let page = parseUrl segments
        let nextModel = { model with CurrentPage = page }

        match page with
        | UserProfile username ->
            // ユーザープロファイルページに遷移した場合、データをロード
            let userModel =
                { nextModel.UserProfileModel with
                    Username = username
                    IsLoading = true }

            { nextModel with
                UserProfileModel = userModel },
            Cmd.ofMsg (UserProfileMsg LoadUserData)
        | _ -> nextModel, Cmd.none

// ナビゲーションビュー
let navigationView (model: Model) (dispatch: Msg -> unit) =
    Html.div
        [ prop.className "navbar"
          prop.children
              [ Html.ul
                    [ prop.className "nav-list"
                      prop.children
                          [ Html.li
                                [ prop.className "nav-item"
                                  prop.children
                                      [ Html.a
                                            [ prop.href (Router.format ([]))
                                              prop.onClick (fun e ->
                                                  e.preventDefault ()
                                                  console.log ("Home リンクがクリックされました")
                                                  dispatch (NavigateTo Home))
                                              prop.text "Home" ] ] ]
                            Html.li
                                [ prop.className "nav-item"
                                  prop.children
                                      [ Html.a
                                            [ prop.href (Router.format ([ "counter" ]))
                                              prop.onClick (fun e ->
                                                  e.preventDefault ()
                                                  console.log ("Counter リンクがクリックされました")
                                                  dispatch (NavigateTo Counter))
                                              prop.text "Counter" ] ] ]
                            Html.li
                                [ prop.className "nav-item"
                                  prop.children
                                      [ Html.a
                                            [ prop.href (Router.format ([ "user"; "john" ]))
                                              prop.onClick (fun e ->
                                                  e.preventDefault ()
                                                  console.log ("John リンクがクリックされました")
                                                  dispatch (NavigateTo(UserProfile "john")))
                                              prop.text "John's Profile" ] ] ]
                            Html.li
                                [ prop.className "nav-item"
                                  prop.children
                                      [ Html.a
                                            [ prop.href (Router.format ([ "user"; "alice" ]))
                                              prop.onClick (fun e ->
                                                  e.preventDefault ()
                                                  console.log ("Alice リンクがクリックされました")
                                                  dispatch (NavigateTo(UserProfile "alice")))
                                              prop.text "Alice's Profile" ] ] ] ] ] ] ]

// ホームページビュー
let homeView =
    Html.div
        [ prop.className "page"
          prop.children [ Html.h1 "Home"; Html.p "Welcome to the Elmish Router Sample App" ] ]

// カウンターページビュー
let counterView (model: CounterModel) (dispatch: CounterMsg -> unit) =
    Html.div
        [ prop.className "page"
          prop.children
              [ Html.h1 "Counter"
                Html.p (sprintf "Current count: %d" model.Count)
                Html.button [ prop.onClick (fun _ -> dispatch Increment); prop.text "+" ]
                Html.button [ prop.onClick (fun _ -> dispatch Decrement); prop.text "-" ] ] ]

// ユーザープロファイルページビュー
let userProfileView (model: UserProfileModel) (dispatch: UserProfileMsg -> unit) =
    Html.div
        [ prop.className "page"
          prop.children
              [ Html.h1 (sprintf "%s's Profile" model.Username)
                if model.IsLoading then
                    Html.p "Loading..."
                else
                    Html.div
                        [ prop.children
                              [ Html.p (sprintf "Username: %s" model.Username)
                                Html.p "Profile data loaded successfully!" ] ] ] ]

// メインビュー
let view (model: Model) (dispatch: Msg -> unit) =
    React.router
        [ router.onUrlChanged (fun segments ->
              console.log ("URL変更検出:", segments)
              dispatch (UrlChanged segments))
          router.children
              [ Html.div
                    [ prop.className "app-container"
                      prop.children
                          [ navigationView model dispatch
                            Html.div
                                [ prop.className "content"
                                  prop.children
                                      [ match model.CurrentPage with
                                        | Home -> homeView
                                        | Counter -> counterView model.CounterModel (CounterMsg >> dispatch)
                                        | UserProfile _ ->
                                            userProfileView model.UserProfileModel (UserProfileMsg >> dispatch) ] ]
                            // CSS スタイル
                            Html.style
                                """
                        .app-container { font-family: Arial, sans-serif; max-width: 800px; margin: 0 auto; padding: 20px; }
                        .navbar { background-color: #f0f0f0; padding: 10px; margin-bottom: 20px; border-radius: 5px; }
                        .nav-list { display: flex; list-style: none; padding: 0; margin: 0; }
                        .nav-item { margin-right: 20px; }
                        .nav-item a { text-decoration: none; color: #333; }
                        .nav-item a:hover { text-decoration: underline; }
                        .content { padding: 20px; background-color: #f9f9f9; border-radius: 5px; }
                        .page { min-height: 200px; }
                        button { margin: 0 5px; padding: 5px 10px; }
                    """ ] ] ] ]

// アプリケーションのエントリーポイント
console.log ("アプリケーション開始")

Program.mkProgram init update view
|> Program.withReactSynchronous "elmish-app"
|> Program.withConsoleTrace
|> Program.run

console.log ("Program.run完了")
