namespace UI.Pages

open Feliz
open Feliz.DaisyUI
open Domain.ValueObjects.User
open UI.State.ViewModels
open UI.State.Messages
open Shared.I18n.TranslationService
open Domain.Errors
open UI.Components.Atoms.ErrorDisplay

/// ログインページコンポーネント
module LoginPage =
    // ユーザー名入力フィールドコンポーネント
    [<ReactComponent>]
    let private UsernameField
        (username: string)
        (language: Language)
        (error: IError option)
        (onChanged: string -> unit)
        =
        Daisy.fieldset
            [ prop.className "mt-4"
              prop.children
                  [ Daisy.fieldsetLabel [ prop.htmlFor "username"; prop.text (getText language Username) ]

                    Daisy.input
                        [ prop.id "username"
                          prop.className (
                              if error.IsSome then
                                  "w-full input input-bordered input-error"
                              else
                                  "w-full input input-bordered"
                          )
                          prop.type' "text"
                          prop.placeholder (
                              if language = Japanese then
                                  "ユーザー名を入力してください"
                              else
                                  "Enter your username"
                          )
                          prop.value username
                          prop.onChange onChanged
                          prop.required true ]

                    // フィールド固有のエラー表示
                    match error with
                    | Some err -> ErrorView (Some err) language
                    | None -> Html.none ] ]

    // パスワード入力フィールドコンポーネント
    [<ReactComponent>]
    let private PasswordField
        (password: string)
        (language: Language)
        (error: IError option)
        (onChanged: string -> unit)
        =
        Daisy.fieldset
            [ prop.className "mt-4"
              prop.children
                  [ Daisy.fieldsetLabel [ prop.htmlFor "password"; prop.text (getText language Password) ]

                    Daisy.input
                        [ prop.id "password"
                          prop.className (
                              if error.IsSome then
                                  "w-full input input-bordered input-error"
                              else
                                  "w-full input input-bordered"
                          )
                          prop.type' "password"
                          prop.placeholder (
                              if language = Japanese then
                                  "パスワードを入力してください"
                              else
                                  "Enter your password"
                          )
                          prop.value password
                          prop.onChange onChanged
                          prop.required true ]

                    // フィールド固有のエラー表示
                    match error with
                    | Some err -> ErrorView (Some err) language
                    | None -> Html.none ] ]

    // 言語選択コンポーネント
    [<ReactComponent>]
    let private LanguageSelector (currentLanguage: Language) (onChanged: Language -> unit) =
        let languageOptions =
            [ Language.English, Language.ToDisplayName Language.English
              Language.Japanese, Language.ToDisplayName Language.Japanese ]

        Daisy.fieldset
            [ prop.className "mt-4"
              prop.children
                  [ Daisy.fieldsetLabel [ prop.htmlFor "language"; prop.text (getText currentLanguage Language) ]

                    Daisy.select
                        [ prop.id "language"
                          prop.className "w-full select select-bordered"
                          prop.value (Language.ToDisplayName currentLanguage)
                          prop.onChange (fun value ->
                              let selectedLang =
                                  languageOptions
                                  |> List.find (fun (_, displayName) -> displayName = value)
                                  |> fst

                              onChanged selectedLang)
                          prop.children
                              [ for (lang, displayName) in languageOptions do
                                    Html.option [ prop.value displayName; prop.text displayName ] ] ] ] ]

    // ログインカードの内容
    [<ReactComponent>]
    let private LoginCardContent (state: LoginPageState) (dispatch: LoginMsg -> unit) =
        let getText = getText state.Language

        let handleSubmit (e: Browser.Types.Event) =
            e.preventDefault ()
            dispatch LoginSubmit

        // フィールド固有のエラーを特定
        let usernameError, passwordError =
            match state.Error with
            | Some err ->
                match err with
                | :? DomainError as domainErr ->
                    match domainErr.Details with
                    | ValidationError(field, _, _) ->
                        if field = "username" then Some err, None
                        elif field = "password" then None, Some err
                        else None, None
                    | _ -> None, None
                | _ -> None, None
            | None -> None, None

        Html.div
            [ Html.h2 [ prop.className "text-2xl font-bold text-center"; prop.text (getText Login) ]

              // フォーム全体のエラー表示
              ErrorView state.Error state.Language

              Html.form
                  [ prop.onSubmit handleSubmit
                    prop.children
                        [
                          // ユーザー名入力
                          UsernameField state.Username state.Language usernameError (SetUsername >> dispatch)

                          // パスワード入力
                          PasswordField state.Password state.Language passwordError (SetPassword >> dispatch)

                          // 言語選択
                          LanguageSelector state.Language (SetLanguage >> dispatch)

                          // ログインボタン
                          Daisy.button.button
                              [ prop.className "w-full mt-6"
                                prop.type' "submit"
                                button.primary
                                // 送信中はローディング表示
                                if state.IsSubmitting then button.loading else ()
                                prop.text (getText Submit) ] ] ] ]

    // メインのログインページコンポーネント
    [<ReactComponent>]
    let LoginPage (state: LoginPageState) (dispatch: LoginMsg -> unit) =
        Daisy.hero
            [ prop.className "min-h-screen bg-base-200"
              prop.children
                  [ Daisy.heroContent
                        [ prop.className "text-center"
                          prop.children
                              [ Html.div
                                    [ prop.className "max-w-md mx-auto"
                                      prop.children
                                          [ Daisy.card
                                                [ prop.className "shadow-xl bg-base-100"
                                                  prop.children [ Daisy.cardBody [ LoginCardContent state dispatch ] ] ] ] ] ] ] ] ]
