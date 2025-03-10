namespace UI.Pages

open Feliz
open Feliz.DaisyUI
open Domain.ValueObjects.User
open UI.State.Types
open Shared.I18n.TranslationService
open Domain.Errors

/// Login page component
module LoginPage =
    // エラー表示コンポーネント
    [<ReactComponent>]
    let private ErrorMessageView (errorKey: ResourceKey option) (language: Language) =
        match errorKey with
        | Some key ->
            Daisy.alert [ 
                alert.error
                prop.className "mt-4"
                prop.children [ Html.span [ prop.text (getText language key) ] ] 
            ]
        | None -> Html.none

    // ROP エラー表示コンポーネント
    [<ReactComponent>]
    let private RailwayErrorView (error: Error option) =
        match error with
        | Some err ->
            let errorMsg = ErrorHelpers.toUserMessage err
            let errorCode = ErrorHelpers.toErrorCode err
            
            Daisy.alert [
                alert.error
                prop.className "mt-4"
                prop.children [
                    Html.div [
                        prop.className "flex justify-between items-center"
                        prop.children [
                            Html.span [ prop.text errorMsg ]
                            Html.span [
                                prop.className "text-xs opacity-70"
                                prop.text (sprintf "エラーコード: %s" errorCode)
                            ]
                        ]
                    ]
                ]
            ]
        | None -> Html.none

    // ユーザー名入力フィールドコンポーネント
    [<ReactComponent>]
    let private UsernameField (username: string) (language: Language) (onChanged: string -> unit) =
        Daisy.fieldset [
            prop.className "mt-4"
            prop.children [
                Daisy.fieldsetLabel [
                    prop.htmlFor "username"
                    prop.text (getText language Username)
                ]
                
                Daisy.input [
                    prop.id "username"
                    prop.className "w-full input input-bordered"
                    prop.type' "text"
                    prop.placeholder (
                        if language = Japanese then "ユーザー名を入力してください"
                        else "Enter your username"
                    )
                    prop.value username
                    prop.onChange onChanged
                    prop.required true
                ]
            ]
        ]

    // パスワード入力フィールドコンポーネント
    [<ReactComponent>]
    let private PasswordField (password: string) (language: Language) (onChanged: string -> unit) =
        Daisy.fieldset [
            prop.className "mt-4"
            prop.children [
                Daisy.fieldsetLabel [
                    prop.htmlFor "password"
                    prop.text (getText language Password)
                ]
                
                Daisy.input [
                    prop.id "password"
                    prop.className "w-full input input-bordered"
                    prop.type' "password"
                    prop.placeholder (
                        if language = Japanese then "パスワードを入力してください"
                        else "Enter your password"
                    )
                    prop.value password
                    prop.onChange onChanged
                    prop.required true
                ]
            ]
        ]

    // 言語選択コンポーネント
    [<ReactComponent>]
    let private LanguageSelector (currentLanguage: Language) (onChanged: Language -> unit) =
        let languageOptions = [
            Language.English, Language.ToDisplayName Language.English
            Language.Japanese, Language.ToDisplayName Language.Japanese
        ]
        
        Daisy.fieldset [
            prop.className "mt-4"
            prop.children [
                Daisy.fieldsetLabel [
                    prop.htmlFor "language"
                    prop.text (getText currentLanguage Language)
                ]
                
                Daisy.select [
                    prop.id "language"
                    prop.className "w-full select select-bordered"
                    prop.value (Language.ToDisplayName currentLanguage)
                    prop.onChange (fun value ->
                        let selectedLang =
                            languageOptions
                            |> List.find (fun (_, displayName) -> displayName = value)
                            |> fst
                        onChanged selectedLang
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

    // ログインカードの内容
    [<ReactComponent>]
    let private LoginCardContent (model: LoginModel) (dispatch: LoginMsg -> unit) =
        let getText = getText model.Language
        
        let handleSubmit (e: Browser.Types.Event) =
            e.preventDefault()
            dispatch LoginSubmit
        
        Html.div [
            Html.h2 [
                prop.className "text-2xl font-bold text-center"
                prop.text (getText Login)
            ]
            
            // エラーメッセージ表示
            ErrorMessageView model.ErrorMessage model.Language
            RailwayErrorView model.Error
            
            Html.form [
                prop.onSubmit handleSubmit
                prop.children [
                    // ユーザー名入力
                    UsernameField 
                        model.Username 
                        model.Language 
                        (SetUsername >> dispatch)
                    
                    // パスワード入力
                    PasswordField 
                        model.Password 
                        model.Language 
                        (SetPassword >> dispatch)
                    
                    // 言語選択
                    LanguageSelector 
                        model.Language 
                        (SetLanguage >> dispatch)
                    
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

    // メインのログインページコンポーネント
    [<ReactComponent>]
    let LoginPage (model: LoginModel) (dispatch: LoginMsg -> unit) =
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
                                            LoginCardContent model dispatch
                                        ]
                                    ]
                                ]
                            ]
                        ]
                    ]
                ]
            ]
        ]