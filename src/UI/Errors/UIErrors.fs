namespace UI.Errors

open Domain.Errors
open Common.Utilities

/// UI層のエラー詳細
type UIErrorDetails =
    | MissingInput of fieldName: string
    | InvalidSelection of selection: string
    | FormError of messageKey: string * parameters: Map<string, string>

/// UI層のエラー実装
type UIError =
    { Details: UIErrorDetails
      ErrorContext: Map<string, string> option }

    interface IError with
        member this.Code =
            match this.Details with
            | MissingInput _ -> "UI-MI-001"
            | InvalidSelection _ -> "UI-IS-001"
            | FormError _ -> "UI-FRM-001"

        member this.MessageKey =
            match this.Details with
            | MissingInput _ -> "error.field.required"
            | InvalidSelection _ -> "error.invalid.selection"
            | FormError(key, _) -> key

        member this.MessageParams =
            match this.Details with
            | MissingInput field -> Map [ ("field", field) ]
            | InvalidSelection selection -> Map [ ("selection", selection) ]
            | FormError(_, parameters) -> parameters

        member this.Context = this.ErrorContext

        member this.WithContext contextMap =
            let newContext = ErrorUtils.mergeContexts this.ErrorContext contextMap
            { this with ErrorContext = newContext } :> IError

/// UIエラー操作のためのヘルパー関数
module UIErrorHelpers =

    /// 必須フィールドのエラーを作成
    let createMissingInputError fieldName : IError =
        { UIError.Details = MissingInput fieldName
          ErrorContext = None }
        :> IError

    /// 無効な選択のエラーを作成
    let createInvalidSelectionError selection : IError =
        { UIError.Details = InvalidSelection selection
          ErrorContext = None }
        :> IError

    /// フォームエラーを作成
    let createFormError messageKey parameters : IError =
        { UIError.Details = FormError(messageKey, parameters)
          ErrorContext = None }
        :> IError
        
    /// エラーにコンテキスト情報を追加
    let withContext (error: IError) (contextMap: Map<string, string>) : IError =
        error.WithContext contextMap