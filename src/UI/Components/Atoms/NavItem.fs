namespace UI.Components.Atoms

open System
open Feliz

/// Navigation item component
module NavItem =

    open Feliz
    open Feliz.Router

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