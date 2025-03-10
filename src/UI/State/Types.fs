namespace UI.State

open Domain.ValueObjects.Types
open Domain.ValueObjects.User
open Shared.I18n.TranslationService

/// Application state and message types
module Types =
    type LoginModel =
        { Username: string
          Password: string
          Language: Language
          ErrorMessage: ResourceKey option }

    type Model =
        { Counter: CounterModel
          UserProfile: UserProfileModel
          CurrentUrl: string list
          CurrentPage: Page
          Login: LoginModel
          CurrentUser: UserProfile option }

    type LoginMsg =
        | SetUsername of string
        | SetPassword of string
        | SetLanguage of Language
        | LoginSubmit
        | LoginSuccess of UserProfile
        | LoginFailed of string

    type CounterMsg =
        | Increment
        | Decrement

    type UserProfileMsg =
        | LoadUserData of string
        | UserDataLoaded

    type Msg =
        | LoginMsg of LoginMsg
        | CounterMsg of CounterMsg
        | UserProfileMsg of UserProfileMsg
        | UrlChanged of string list
        | Logout
