namespace UI.Errors

open Domain.Errors

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
            let newContext =
                match this.ErrorContext with
                | Some existingContext -> Some(Map.fold (fun acc k v -> Map.add k v acc) existingContext contextMap)
                | None -> Some contextMap

            { this with ErrorContext = newContext } :> IError

/// UIエラー操作のためのヘルパー関数
module UIErrorHelpers =

    /// 必須フィールドのエラーを作成
    let missingInput fieldName : IError =
        { UIError.Details = MissingInput fieldName
          ErrorContext = None }
        :> IError

    /// 無効な選択のエラーを作成
    let invalidSelection selection : IError =
        { UIError.Details = InvalidSelection selection
          ErrorContext = None }
        :> IError

    /// フォームエラーを作成
    let formError messageKey parameters : IError =
        { UIError.Details = FormError(messageKey, parameters)
          ErrorContext = None }
        :> IError
