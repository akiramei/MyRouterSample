namespace UI.State

open Domain.ValueObjects.Localization
open Domain.ValueObjects.User
open Domain.Errors

/// アプリケーションメッセージ
module Messages =

    /// ホームページのメッセージ
    type HomePageMsg =
        | LoadData
        | DataLoaded
        | DataError of IError

    /// カウンターページのメッセージ
    type CounterMsg =
        | Increment
        | Decrement
        | Reset

    /// ユーザープロファイルページのメッセージ
    type UserProfileMsg =
        | LoadUserData of userId: string
        | UserDataLoaded of UserProfile
        | UserDataError of IError
        | StartEditing
        | CancelEditing
        | SaveProfile
        | ProfileSaved
        | ProfileError of IError

    /// ログインページのメッセージ
    type LoginMsg =
        | SetUsername of string
        | SetPassword of string
        | SetLanguage of Language
        | LoginSubmit
        | LoginSuccess of UserProfile
        | LoginFailed of IError

    /// アプリケーション全体のメッセージ
    type AppMsg =
        | HomeMsg of HomePageMsg
        | CounterMsg of CounterMsg
        | UserProfileMsg of UserProfileMsg
        | LoginMsg of LoginMsg
        | UrlChanged of string list
        | Logout // ログアウトボタンがクリックされた
        | ShowError of string
        | ClearError
