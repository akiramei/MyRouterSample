namespace UI.Errors

open Domain.Errors

/// UIエラー操作のためのヘルパー関数
module UIErrorHelpers =

    /// UIエラーを作成するヘルパー関数
    let asUIError (details: UIErrorDetails) context : IError =
        { UIError.Details = details
          ErrorContext = context }
        :> IError

    /// MissingInputエラーを作成
    let missingInput (fieldName: string) : IError = asUIError (MissingInput fieldName) None

    /// InvalidSelectionエラーを作成
    let invalidSelection (selection: string) : IError =
        asUIError (InvalidSelection selection) None

    /// FormErrorエラーを作成
    let formError (message: string) : IError = asUIError (FormError message) None
