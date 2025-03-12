namespace Common.Platform

/// プラットフォーム固有の機能を抽象化するモジュール
/// 条件付きコンパイル (#if FABLE_COMPILER) を集約することを目的とする
module PlatformServices =
    /// ロギング機能を抽象化
    module Logging =
        /// デバッグレベルのログを出力
        let debug (message: string) : unit =
            #if FABLE_COMPILER
            Browser.Dom.console.debug message
            #else
            System.Diagnostics.Debug.WriteLine(message)
            #endif

        /// 情報レベルのログを出力
        let info (message: string) : unit =
            #if FABLE_COMPILER
            Browser.Dom.console.info message
            #else
            System.Console.WriteLine(message)
            #endif

        /// 警告レベルのログを出力
        let warn (message: string) : unit =
            #if FABLE_COMPILER
            Browser.Dom.console.warn message
            #else
            System.Console.WriteLine($"WARNING: {message}")
            #endif

        /// エラーレベルのログを出力
        let error (message: string) : unit =
            #if FABLE_COMPILER
            Browser.Dom.console.error message
            #else
            System.Console.Error.WriteLine(message)
            #endif

    /// ファイル操作を抽象化
    module FileSystem =
        /// ファイルを非同期で読み込む
        let readFileAsync (path: string) : Async<Result<string, string>> =
            async {
                try
                    #if FABLE_COMPILER
                    // Fable環境ではHTTP経由でファイルを取得
                    let! (statusCode, responseText) = Fable.SimpleHttp.Http.get path
                    if statusCode = 200 then
                        return Ok responseText
                    else
                        return Error $"Failed to load file: Status code {statusCode}"
                    #else
                    // .NET環境では直接ファイルを読み込む
                    let! content = System.IO.File.ReadAllTextAsync(path) |> Async.AwaitTask
                    return Ok content
                    #endif
                with ex ->
                    return Error ex.Message
            }

    /// 環境情報を提供
    module Environment =
        /// 開発環境かどうかを判定
        let isDevelopment : bool =
            #if DEBUG
            true
            #else
            false
            #endif

        /// 実行環境の種類を取得
        let runtime : string =
            #if FABLE_COMPILER
            "Fable"
            #else
            ".NET"
            #endif

    /// 例外処理を抽象化
    module ExceptionHandling =
        /// 例外から適切なコンテキスト情報を抽出
        let getExceptionContext (ex: System.Exception) : Map<string, string> =
            #if FABLE_COMPILER
            // Fable環境では制限された情報のみ取得可能
            Map [ 
                "exceptionMessage", ex.Message
                "exceptionType", "Unknown" // Fable環境ではGetType()を使用できない
                "timestamp", System.DateTime.Now.ToString()
            ]
            #else
            // .NET環境ではより詳細な情報を取得
            Map [
                "exceptionType", ex.GetType().Name
                "exceptionMessage", ex.Message
                "stackTrace", if isNull ex.StackTrace then "" else ex.StackTrace
                "timestamp", System.DateTime.Now.ToString()
            ]
            #endif