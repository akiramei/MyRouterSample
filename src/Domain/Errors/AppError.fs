namespace Domain.Errors

/// Application error definitions
module AppError =

// Define application errors
    type AppError =
        | RouteNotFound of path: string
        | UserDataLoadError of username: string
        | GenericError of message: string