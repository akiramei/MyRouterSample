namespace UI.Pages

open Feliz
open Feliz.DaisyUI

/// 404 Not Found page component
module NotFoundPage =

    open Feliz
    open Feliz.DaisyUI

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