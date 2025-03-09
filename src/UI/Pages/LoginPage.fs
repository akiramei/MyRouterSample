namespace UI.Pages

open Feliz
open Feliz.DaisyUI
open Domain.ValueObjects.User
open UI.State.Types
open Shared.I18n.Resources

/// Login page component
module LoginPage =
    [<ReactComponent>]
    let LoginPage (model: LoginModel) (dispatch: LoginMsg -> unit) =
        // 言語に応じたテキストを取得
        let getText : ResourceKey -> string = getText model.Language
        
        let handleSubmit (e: Browser.Types.Event) =
            e.preventDefault()
            dispatch LoginSubmit
        
        let languageOptions = [
            Language.English, Language.ToDisplayName Language.English
            Language.Japanese, Language.ToDisplayName Language.Japanese
        ]

        Daisy.hero [
            prop.className "min-h-screen bg-base-200"
            prop.children [
                Daisy.heroContent [
                    prop.className "text-center"
                    prop.children [
                        Html.div [
                            prop.className "max-w-md mx-auto"
                            prop.children [
                                Daisy.card [
                                    prop.className "shadow-xl bg-base-100"
                                    prop.children [
                                        Daisy.cardBody [
                                            Html.h2 [
                                                prop.className "text-2xl font-bold text-center"
                                                prop.text (getText Login)
                                            ]
                                            
                                            // エラーメッセージ表示
                                            match model.ErrorMessage with
                                            | Some errorKey ->
                                                Daisy.alert [
                                                    alert.error
                                                    prop.className "mt-4"
                                                    prop.children [
                                                        Html.span [ prop.text (getText errorKey) ]
                                                    ]
                                                ]
                                            | None -> Html.none
                                            
                                            Html.form [
                                                prop.onSubmit handleSubmit
                                                prop.children [
                                                    // ユーザー名入力フィールド
                                                    Daisy.fieldset [
                                                        prop.className "mt-4"
                                                        prop.children [
                                                            Daisy.fieldsetLabel [
                                                                prop.htmlFor "username"
                                                                prop.text (getText Username)
                                                            ]
                                                            
                                                            Daisy.input [
                                                                prop.id "username"
                                                                prop.className "w-full input input-bordered"
                                                                prop.type' "text"
                                                                prop.placeholder (
                                                                    if model.Language = Japanese then
                                                                        "ユーザー名を入力してください" 
                                                                    else 
                                                                        "Enter your username"
                                                                )
                                                                prop.value model.Username
                                                                prop.onChange (SetUsername >> dispatch)
                                                                prop.required true
                                                            ]
                                                        ]
                                                    ]
                                                    
                                                    // パスワード入力フィールド
                                                    Daisy.fieldset [
                                                        prop.className "mt-4"
                                                        prop.children [
                                                            Daisy.fieldsetLabel [
                                                                prop.htmlFor "password"
                                                                prop.text (getText Password)
                                                            ]
                                                            
                                                            Daisy.input [
                                                                prop.id "password"
                                                                prop.className "w-full input input-bordered"
                                                                prop.type' "password"
                                                                prop.placeholder (
                                                                    if model.Language = Japanese then
                                                                        "パスワードを入力してください" 
                                                                    else 
                                                                        "Enter your password"
                                                                )
                                                                prop.value model.Password
                                                                prop.onChange (SetPassword >> dispatch)
                                                                prop.required true
                                                            ]
                                                        ]
                                                    ]
                                                    
                                                    // 言語選択
                                                    Daisy.fieldset [
                                                        prop.className "mt-4"
                                                        prop.children [
                                                            Daisy.fieldsetLabel [
                                                                prop.htmlFor "language"
                                                                prop.text (getText Language)
                                                            ]
                                                            
                                                            Daisy.select [
                                                                prop.id "language"
                                                                prop.className "w-full select select-bordered"
                                                                prop.value (Language.ToDisplayName model.Language)
                                                                prop.onChange (fun value ->
                                                                    let selectedLang = 
                                                                        languageOptions
                                                                        |> List.find (fun (_, displayName) -> displayName = value)
                                                                        |> fst
                                                                    dispatch (SetLanguage selectedLang)
                                                                )
                                                                prop.children [
                                                                    for (lang, displayName) in languageOptions do
                                                                        Html.option [
                                                                            prop.value displayName
                                                                            prop.text displayName
                                                                        ]
                                                                ]
                                                            ]
                                                        ]
                                                    ]
                                                    
                                                    // ログインボタン
                                                    Daisy.button.button [
                                                        prop.className "w-full mt-6"
                                                        prop.type' "submit"
                                                        button.primary
                                                        prop.text (getText Submit)
                                                    ]
                                                ]
                                            ]
                                        ]
                                    ]
                                ]
                            ]
                        ]
                    ]
                ]
            ]
        ]