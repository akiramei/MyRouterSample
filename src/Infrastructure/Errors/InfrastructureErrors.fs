namespace Infrastructure.Errors

open Domain.Errors

/// インフラストラクチャ層のエラー詳細
type InfrastructureErrorDetails =
    | NetworkError of message: string
    | AuthenticationError of message: string
    | AuthorizationError of message: string
    | SystemError of message: string

/// インフラストラクチャ層のエラー実装
type InfrastructureError =
    { Details: InfrastructureErrorDetails
      ErrorContext: Map<string, string> option }

    interface IError with
        member this.Category = "Infrastructure"

        member this.Code =
            match this.Details with
            | NetworkError _ -> "INF-NET-001"
            | AuthenticationError _ -> "INF-AUTH-001"
            | AuthorizationError _ -> "INF-AUTHZ-001"
            | SystemError _ -> "INF-SYS-001"

        member this.UserMessage =
            match this.Details with
            | NetworkError message -> sprintf "ネットワークエラー: %s" message
            | AuthenticationError message -> sprintf "認証エラー: %s" message
            | AuthorizationError message -> sprintf "権限エラー: %s" message
            | SystemError message -> sprintf "システムエラー: %s" message

        member this.Context = this.ErrorContext

        member this.WithContext contextMap =
            let newContext =
                match this.ErrorContext with
                | Some existingContext -> Some(Map.fold (fun acc k v -> Map.add k v acc) existingContext contextMap)
                | None -> Some contextMap

            { this with ErrorContext = newContext } :> IError
