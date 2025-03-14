namespace UI.Pages

open Feliz
open Feliz.DaisyUI
open UI.State.Messages
open UI.State.ViewModels

/// Counter page component with increment/decrement functionality
module CounterPage =

    let CounterPage (model: CounterPageState) (dispatch: CounterMsg -> unit) =
        Daisy.card
            [ prop.className "shadow-lg bg-base-100 border"
              prop.children
                  [ Daisy.cardBody
                        [ Html.h2 [ prop.className "card-title"; prop.text "Counter" ]
                          Html.p [ prop.className "py-4"; prop.text (sprintf "Current count: %d" model.Count) ]
                          Daisy.cardActions
                              [ Html.div
                                    [ Daisy.join
                                          [ Daisy.button.button
                                                [ prop.className "btn-primary btn-outline"
                                                  join.item
                                                  prop.onClick (fun _ -> dispatch Increment)
                                                  prop.text "+" ]
                                            Daisy.button.button
                                                [ prop.className "btn-secondary btn-outline"
                                                  join.item
                                                  prop.onClick (fun _ -> dispatch Decrement)
                                                  prop.text "-" ] ] ] ] ] ] ]
