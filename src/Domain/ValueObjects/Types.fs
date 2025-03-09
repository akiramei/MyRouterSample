namespace Domain.ValueObjects

/// Core domain types and models
module Types =
    type Page =
        | Login
        | Home
        | Counter
        | UserProfile of string
        | NotFound

    type CounterModel = { Count: int }

    type UserProfileModel = { 
        Username: string
        IsLoading: bool 
    }