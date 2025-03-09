namespace UI.Components.Templates

open Feliz
open Feliz.Router
open Domain.ValueObjects.Types
open UI.State.Types
open UI.Components.Molecules.Navigation
open UI.Pages.HomePage
open UI.Pages.CounterPage
open UI.Pages.UserProfilePage
open UI.Pages.NotFoundPage
open UI.Pages.LoginPage
open UI.Components.Templates.Layouts

/// Main application layout and content router
module MainLayout =
    [<ReactComponent>]
    let MainContent (model: Model) (dispatch: Msg -> unit) =
        // プロファイル読み込み処理をuseCallbackでメモ化
        let checkAndLoadProfile = React.useCallback((fun username ->
            if model.UserProfile.Username <> username then
                dispatch (UserProfileMsg (LoadUserData username))
        ), [| model.UserProfile.Username :> obj |])
        
        // URL変更ハンドラをuseCallbackでメモ化
        let urlChanged = React.useCallback((fun urlSegments -> 
            dispatch (UrlChanged urlSegments)
        ), [| dispatch :> obj |])
        
        React.router [
            router.hashMode
            router.onUrlChanged urlChanged
            router.children [
                match model.CurrentPage with
                | Login -> 
                    LoginPage 
                        model.Login 
                        (fun msg -> dispatch (LoginMsg msg))
                | Home -> HomePage()
                | Counter -> 
                    CounterPage 
                        model.Counter 
                        (fun msg -> dispatch (CounterMsg msg))
                | UserProfile username -> 
                    checkAndLoadProfile(username)
                    UserProfilePage 
                        username 
                        model.UserProfile.IsLoading
                | NotFound -> NotFoundPage()
            ]
        ]

    [<ReactComponent>]
    let MainLayout (model: Model) (dispatch: Msg -> unit) =
        let content = MainContent model dispatch
        
        match model.CurrentPage with
        | Login -> 
            // 未認証レイアウト（ナビゲーションなし）
            Layouts.UnauthenticatedLayout content
        | _ ->
            // 認証済みレイアウト（ナビゲーションあり）
            Layouts.AuthenticatedLayout model dispatch content