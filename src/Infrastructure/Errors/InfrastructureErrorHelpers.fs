namespace Infrastructure.Errors

open Domain.Errors

/// インフラストラクチャエラー操作のためのヘルパー関数
module InfrastructureErrorHelpers =

    /// インフラストラクチャエラーを作成するヘルパー関数
    let asInfrastructureError (details: InfrastructureErrorDetails) context : IError =
        { InfrastructureError.Details = details
          ErrorContext = context }
        :> IError

    /// NetworkErrorエラーを作成
    let networkError (message: string) : IError =
        asInfrastructureError (NetworkError message) None

    /// AuthenticationErrorエラーを作成
    let authenticationError (message: string) : IError =
        asInfrastructureError (AuthenticationError message) None

    /// AuthorizationErrorエラーを作成
    let authorizationError (message: string) : IError =
        asInfrastructureError (AuthorizationError message) None

    /// SystemErrorエラーを作成
    let systemError (message: string) : IError =
        asInfrastructureError (SystemError message) None

    /// エラーからのコンテキスト情報追加
    let withRequestInfo (error: IError) (url: string) (statusCode: int) : IError =
        let contextMap =
            Map
                [ "url", url
                  "statusCode", string statusCode
                  "timestamp", System.DateTime.Now.ToString() ]

        error.WithContext contextMap

    /// 例外からインフラストラクチャエラーを作成
    let fromException (ex: System.Exception) : IError =
        let contextMap =
            Map
                [ "exceptionType", ex.GetType().Name
                  "exceptionMessage", ex.Message
                  "stackTrace", if isNull ex.StackTrace then "" else ex.StackTrace ]

        asInfrastructureError (SystemError "システムエラーが発生しました") (Some contextMap)

    /// Fable環境用の例外からインフラストラクチャエラーを作成
    let fromFableException (ex: System.Exception) : IError =
        // Fableでは例外のGetTypeが使えないため、簡略化
        let contextMap = Map [ "exceptionMessage", ex.Message ]
        asInfrastructureError (SystemError "システムエラーが発生しました") (Some contextMap)

#if FABLE_COMPILER
    // Fable環境では標準のfromExceptionの代わりにこちらを使用
    let fromError = fromFableException
#else
    // .NET環境では標準のものを使用
    let fromError = fromException
#endif
