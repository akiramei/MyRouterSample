namespace UI.Pages

/// User profile page component
module UserProfilePage =

    open Feliz
    open Feliz.DaisyUI

    let UserProfilePage (username: string) (isLoading: bool) =
        Daisy.card
            [ prop.className "shadow-lg bg-base-100 border"
              prop.children
                  [ Daisy.cardBody
                        [ Html.h2 [ prop.className "card-title"; prop.text (sprintf "%s's Profile" username) ]

                          if isLoading then
                              Html.div
                                  [ prop.className "flex justify-center py-4"
                                    prop.children [ Daisy.loading [ prop.className "loading-spinner loading-lg" ] ] ]
                          else
                              Html.p [ prop.className "py-4"; prop.text (sprintf "Username: %s" username) ] ] ] ]
