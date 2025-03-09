namespace UI.Components.Molecules

open Feliz
open Feliz.DaisyUI
open Domain.ValueObjects.User
open UI.Services.Router
open UI.Components.Atoms.NavItem
open Shared.I18n.Resources

/// Navigation bar component
module Navigation =
    [<ReactComponent>]
    let Navigation (currentUrl: string list) (currentUser: Domain.ValueObjects.User.UserProfile option) (dispatch: UI.State.Types.Msg -> unit) =
        let isActive = getActiveClass currentUrl
        
        // メモ化してレンダリングパフォーマンスを改善
        let memoizedNavbar = React.memo(fun () ->
            Daisy.navbar [
                prop.className "mb-4 shadow-lg bg-base-200 rounded-box"
                prop.children [
                    Html.div [
                        prop.className "flex-1 px-2"
                        prop.children [
                            Daisy.menu [
                                prop.className "menu-horizontal px-1"
                                prop.children [
                                    NavItem "home" (
                                        match currentUser with
                                        | Some user -> getText user.Language Home
                                        | None -> "Home"
                                    ) (isActive "home") // ホームへのパスを一貫して"home"に変更
                                    NavItem "counter" (
                                        match currentUser with
                                        | Some user -> getText user.Language Counter
                                        | None -> "Counter"
                                    ) (isActive "counter")
                                    NavItem "user/john" "John's Profile" (isActive "user/john")
                                    NavItem "user/alice" "Alice's Profile" (isActive "user/alice")
                                ]
                            ]
                        ]
                    ]
                    
                    // ユーザー情報とログアウトボタン
                    match currentUser with
                    | Some user ->
                        Html.div [
                            prop.className "flex-none gap-2"
                            prop.children [
                                Html.span [
                                    prop.className "mr-2 text-sm font-medium"
                                    prop.text (
                                        match currentUser with 
                                        | Some user -> sprintf "%s, %s" (getText user.Language Welcome) user.Username
                                        | None -> ""
                                    )
                                ]
                                
                                Daisy.button.button [
                                    prop.className "btn-sm"
                                    button.ghost
                                    prop.onClick (fun _ -> dispatch UI.State.Types.Logout)
                                    prop.children [
                                        Html.text (
                                            match currentUser with
                                            | Some user -> getText user.Language Logout
                                            | None -> "Logout"
                                        )
                                    ]
                                ]
                            ]
                        ]
                    | None -> Html.none
                ]
            ]
        )
        
        memoizedNavbar()