namespace UI.State

open Domain.ValueObjects.Types

/// Application state and message types
module Types =

// Application state and messages
    type Model = {
        Counter: CounterModel
        UserProfile: UserProfileModel
        CurrentUrl: string list
        CurrentPage: Page
    }

    type CounterMsg =
        | Increment
        | Decrement

    type UserProfileMsg =
        | LoadUserData of string
        | UserDataLoaded

    type Msg =
        | CounterMsg of CounterMsg
        | UserProfileMsg of UserProfileMsg
        | UrlChanged of string list