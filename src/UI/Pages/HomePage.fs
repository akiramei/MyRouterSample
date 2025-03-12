namespace UI.Pages

open Feliz
open Feliz.DaisyUI

/// Home page component
module HomePage =

    open Feliz
    open Feliz.DaisyUI

    [<ReactComponent>]
    let HomePage () =
        Daisy.hero
            [ prop.className "bg-primary rounded-box"
              prop.children
                  [ Daisy.heroContent
                        [ prop.className "text-center"
                          prop.children
                              [ Html.h1 [ prop.className "text-4xl font-bold text-primary-content"; prop.text "Home" ]
                                Html.p
                                    [ prop.className "py-6 text-primary-content"
                                      prop.text "Welcome to the Elmish Router Sample App" ] ] ] ] ]
