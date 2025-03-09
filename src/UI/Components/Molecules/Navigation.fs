namespace UI.Components.Molecules

open Feliz
open Feliz.DaisyUI
open UI.Services.Router
open UI.Components.Atoms.NavItem

/// Navigation bar component
module Navigation =
    [<ReactComponent>]
    let Navigation (currentUrl: string list) =
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
        )
        
        memoizedNavbar()