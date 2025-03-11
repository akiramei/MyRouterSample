namespace UI.Errors

open Domain.Errors

/// UI層のエラー詳細
type UIErrorDetails =
    | MissingInput of fieldName: string
    | InvalidSelection of selection: string
    | FormError of message: string

/// UI層のエラー実装
type UIError =
    { Details: UIErrorDetails
      ErrorContext: Map<string, string> option }

    interface IError with
        member this.Category = "UI"

        member this.Code =
            match this.Details with
            | MissingInput _ -> "UI-MI-001"
            | InvalidSelection _ -> "UI-IS-001"
            | FormError _ -> "UI-FRM-001"

        member this.UserMessage =
            match this.Details with
            | MissingInput field -> sprintf "%s は必須です" field
            | InvalidSelection selection -> sprintf "選択された値 '%s' は無効です" selection
            | FormError message -> sprintf "フォームエラー: %s" message

        member this.Context = this.ErrorContext

        member this.WithContext contextMap =
            let newContext =
                match this.ErrorContext with
                | Some existingContext -> Some(Map.fold (fun acc k v -> Map.add k v acc) existingContext contextMap)
                | None -> Some contextMap

            { this with ErrorContext = newContext } :> IError
