namespace UI.State

open Domain.ValueObjects.Types
open Domain.ValueObjects.User
open Domain.Errors

/// Application state and message types
module Types =
    // エラー表示用のメッセージ型
    type ErrorDisplay =
        { IsVisible: bool
          Message: string option }

    type LoginModel =
        { Username: string
          Password: string
          Language: Language
          ErrorMessage: Shared.I18n.TranslationService.ResourceKey option
          // エラー処理用のフィールド
          Error: IError option }

    type Model =
        { Counter: CounterModel
          UserProfile: UserProfileModel
          CurrentUrl: string list
          CurrentPage: Page
          Login: LoginModel
          CurrentUser: UserProfile option
          // グローバルエラー表示状態
          ErrorDisplay: ErrorDisplay }

    type LoginMsg =
        | SetUsername of string
        | SetPassword of string
        | SetLanguage of Language
        | LoginSubmit
        | LoginSuccess of UserProfile
        | LoginFailed of IError

    type CounterMsg =
        | Increment
        | Decrement

    type UserProfileMsg =
        | LoadUserData of string
        | UserDataLoaded
        | UserDataError of IError
        | ShowUserProfileError of string // ユーザープロファイル用のエラー表示

    type Msg =
        | LoginMsg of LoginMsg
        | CounterMsg of CounterMsg
        | UserProfileMsg of UserProfileMsg
        | UrlChanged of string list
        | Logout
        | ShowError of string
        | ClearError
