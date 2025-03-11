namespace Infrastructure.Errors

open Domain.Errors

/// インフラストラクチャ層のエラー詳細
type InfrastructureErrorDetails =
    | NetworkError of endpoint: string * message: string
    | AuthenticationError of message: string
    | AuthorizationError of resource: string * action: string
    | SystemError of code: string * message: string

/// インフラストラクチャ層のエラー実装
type InfrastructureError =
    { Details: InfrastructureErrorDetails
      ErrorContext: Map<string, string> option }

    interface IError with
        member this.Code =
            match this.Details with
            | NetworkError _ -> "INF-NET-001"
            | AuthenticationError _ -> "INF-AUTH-001"
            | AuthorizationError _ -> "INF-AUTHZ-001"
            | SystemError(code, _) -> $"INF-SYS-{code}"

        member this.MessageKey =
            match this.Details with
            | NetworkError _ -> "error.network.connection"
            | AuthenticationError _ -> "error.authentication.failed"
            | AuthorizationError _ -> "error.authorization.denied"
            | SystemError _ -> "error.system.generic"

        member this.MessageParams =
            match this.Details with
            | NetworkError(endpoint, message) -> Map [ ("endpoint", endpoint); ("message", message) ]
            | AuthenticationError message -> Map [ ("message", message) ]
            | AuthorizationError(resource, action) -> Map [ ("resource", resource); ("action", action) ]
            | SystemError(code, message) -> Map [ ("code", code); ("message", message) ]

        member this.Context = this.ErrorContext

        member this.WithContext contextMap =
            let newContext =
                match this.ErrorContext with
                | Some existingContext -> Some(Map.fold (fun acc k v -> Map.add k v acc) existingContext contextMap)
                | None -> Some contextMap

            { this with ErrorContext = newContext } :> IError

/// インフラストラクチャエラー操作のためのヘルパー関数
module InfrastructureErrorHelpers =

    /// ネットワークエラーを作成
    let networkError endpoint message : IError =
        { InfrastructureError.Details = NetworkError(endpoint, message)
          ErrorContext = None }
        :> IError

    /// 認証エラーを作成
    let authenticationError message : IError =
        { InfrastructureError.Details = AuthenticationError message
          ErrorContext = None }
        :> IError

    /// 権限エラーを作成
    let authorizationError resource action : IError =
        { InfrastructureError.Details = AuthorizationError(resource, action)
          ErrorContext = None }
        :> IError

    /// システムエラーを作成
    let systemError code message : IError =
        { InfrastructureError.Details = SystemError(code, message)
          ErrorContext = None }
        :> IError

    /// エラーからのコンテキスト情報追加
    let withRequestInfo (error: IError) (url: string) (statusCode: int) : IError =
        let contextMap =
            Map
                [ "url", url
                  "statusCode", string statusCode
                  "timestamp", System.DateTime.Now.ToString() ]

        error.WithContext contextMap

    /// 例外からインフラストラクチャエラーを作成（.NET環境用）
    let fromException (ex: System.Exception) : IError =
        let errorCode = "EX001"
        let errorMessage = ex.Message

        let contextMap =
            Map
                [ "exceptionType", ex.GetType().Name
                  "stackTrace", if isNull ex.StackTrace then "" else ex.StackTrace ]

        { InfrastructureError.Details = SystemError(errorCode, errorMessage)
          ErrorContext = Some contextMap }
        :> IError

    /// 例外からインフラストラクチャエラーを作成（Fable環境用）
    let fromFableException (ex: System.Exception) : IError =
        let errorCode = "EX001"
        let errorMessage = ex.Message

        let contextMap = Map [ "exceptionMessage", errorMessage ]

        { InfrastructureError.Details = SystemError(errorCode, errorMessage)
          ErrorContext = Some contextMap }
        :> IError

#if FABLE_COMPILER
    // Fable環境では簡略化されたバージョンを使用
    let fromError = fromFableException
#else
    // 標準.NET環境ではフル機能版を使用
    let fromError = fromException
#endif
