namespace Infrastructure.Errors

open Domain.Errors
open Common.Platform
open Common.Utilities

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
            let newContext = ErrorUtils.mergeContexts this.ErrorContext contextMap
            { this with ErrorContext = newContext } :> IError

/// インフラストラクチャエラー操作のためのヘルパー関数
module InfrastructureErrorHelpers =

    /// ネットワークエラーを作成
    let createNetworkError endpoint message : IError =
        { InfrastructureError.Details = NetworkError(endpoint, message)
          ErrorContext = None }
        :> IError

    /// 認証エラーを作成
    let createAuthenticationError message : IError =
        { InfrastructureError.Details = AuthenticationError message
          ErrorContext = None }
        :> IError

    /// 権限エラーを作成
    let createAuthorizationError resource action : IError =
        { InfrastructureError.Details = AuthorizationError(resource, action)
          ErrorContext = None }
        :> IError

    /// システムエラーを作成
    let createSystemError code message : IError =
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

    /// 例外からインフラストラクチャエラーを作成（プラットフォーム抽象化）
    let createErrorFromException (ex: System.Exception) : IError =
        let errorCode = "EX001"
        let errorMessage = ex.Message
        
        // プラットフォーム抽象化層を使用して例外のコンテキスト情報を取得
        let contextMap = PlatformServices.ExceptionHandling.getExceptionContext ex

        { InfrastructureError.Details = SystemError(errorCode, errorMessage)
          ErrorContext = Some contextMap }
        :> IError