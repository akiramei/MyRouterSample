namespace Shared.I18n

open System
open System.Collections.Generic
open Domain.ValueObjects.User

#if FABLE_COMPILER
open Fable.SimpleHttp
open Browser
#endif

/// CSV形式の翻訳ファイルを読み込み、管理するサービス
module TranslationService =
    /// リソースキーの定義
    type ResourceKey =
        | Login
        | Username
        | Password
        | Language
        | Submit
        | Welcome
        | Logout
        | Home
        | Counter
        | Profile
        | ValidationError
        | UsernameRequired
        | PasswordRequired
        | LanguageRequired

    /// 内部使用のデフォルトリソース
    module private DefaultResources =
        /// ハードコードされたデフォルト言語リソース
        let getDefault (lang: Language) (key: ResourceKey) : string =
            match lang, key with
            | English, Login -> "Login"
            | Japanese, Login -> "ログイン"

            | English, Username -> "Username"
            | Japanese, Username -> "ユーザー名"

            | English, Password -> "Password"
            | Japanese, Password -> "パスワード"

            | English, Language -> "Language"
            | Japanese, Language -> "言語"

            | English, Submit -> "Login"
            | Japanese, Submit -> "ログイン"

            | English, Welcome -> "Welcome"
            | Japanese, Welcome -> "ようこそ"

            | English, Logout -> "Logout"
            | Japanese, Logout -> "ログアウト"

            | English, Home -> "Home"
            | Japanese, Home -> "ホーム"

            | English, Counter -> "Counter"
            | Japanese, Counter -> "カウンター"

            | English, Profile -> "Profile"
            | Japanese, Profile -> "プロフィール"

            | English, ValidationError -> "Validation Error"
            | Japanese, ValidationError -> "入力エラー"

            | English, UsernameRequired -> "Username is required"
            | Japanese, UsernameRequired -> "ユーザー名は必須です"

            | English, PasswordRequired -> "Password is required"
            | Japanese, PasswordRequired -> "パスワードは必須です"

            | English, LanguageRequired -> "Please select a language"
            | Japanese, LanguageRequired -> "言語を選択してください"

    // 翻訳辞書の型
    type private TranslationDictionary = Dictionary<ResourceKey, Dictionary<Language, string>>

    // グローバルな翻訳辞書
    let mutable private translations = TranslationDictionary()

    // 翻訳辞書の初期化済みフラグ
    let mutable private isInitialized = false

    // リソースキーを文字列から変換する関数
    let private parseKey (keyString: string) : ResourceKey option =
        match keyString with
        | "Login" -> Some ResourceKey.Login
        | "Username" -> Some ResourceKey.Username
        | "Password" -> Some ResourceKey.Password
        | "Language" -> Some ResourceKey.Language
        | "Submit" -> Some ResourceKey.Submit
        | "Welcome" -> Some ResourceKey.Welcome
        | "Logout" -> Some ResourceKey.Logout
        | "Home" -> Some ResourceKey.Home
        | "Counter" -> Some ResourceKey.Counter
        | "Profile" -> Some ResourceKey.Profile
        | "ValidationError" -> Some ResourceKey.ValidationError
        | "UsernameRequired" -> Some ResourceKey.UsernameRequired
        | "PasswordRequired" -> Some ResourceKey.PasswordRequired
        | "LanguageRequired" -> Some ResourceKey.LanguageRequired
        | _ -> None

    // CSVを解析して翻訳辞書にロードする
    let private parseCsvContent (csvContent: string) =
#if FABLE_COMPILER
        Dom.console.debug (sprintf "parseCsvContent %s" csvContent)
#endif

        let lines = csvContent.Split('\n')

        // ヘッダー行の処理
        let headerLine = lines.[0].Trim()
        let headers = headerLine.Split(',')

        // 言語のインデックスを取得
        let enIndex = Array.findIndex (fun h -> h = "en") headers
        let jaIndex = Array.findIndex (fun h -> h = "ja") headers

#if FABLE_COMPILER
        Dom.console.debug (sprintf "Column indices: en=%d, ja=%d" enIndex jaIndex)
#endif

        // 新しい翻訳辞書を作成
        let newTranslations = TranslationDictionary()

        // 各行を処理
        for i in 1 .. lines.Length - 1 do
            if i < lines.Length && not (String.IsNullOrWhiteSpace(lines.[i])) then
                let line = lines.[i].Trim()
                let values = line.Split(',')

                if values.Length > 1 then
                    let keyString = values.[0]

                    match parseKey keyString with
                    | Some key ->
                        let langDict = Dictionary<Language, string>()

                        // 英語の翻訳
                        if enIndex < values.Length then
                            langDict.Add(Language.English, values.[enIndex])

                        // 日本語の翻訳
                        if jaIndex < values.Length then
                            langDict.Add(Language.Japanese, values.[jaIndex])

                        newTranslations.Add(key, langDict)
#if FABLE_COMPILER
                        Dom.console.debug (
                            sprintf
                                "Added key: %A - en:%s, ja:%s"
                                key
                                (if langDict.ContainsKey(Language.English) then
                                     langDict.[Language.English]
                                 else
                                     "missing")
                                (if langDict.ContainsKey(Language.Japanese) then
                                     langDict.[Language.Japanese]
                                 else
                                     "missing")
                        )
#endif
                    | None ->
                        // 不明なキーは無視
#if FABLE_COMPILER
                        Dom.console.warn (sprintf "Unknown key: %s" keyString)
#endif
                        ()

        // グローバル辞書を更新
        translations <- newTranslations
        isInitialized <- true
#if FABLE_COMPILER
        Dom.console.debug (
            sprintf "Dictionary initialized with %d entries. isInitialized=%b" newTranslations.Count isInitialized
        )
#endif

    /// 翻訳を取得する関数
    let getText (lang: Language) (key: ResourceKey) : string =
#if FABLE_COMPILER
        Dom.console.debug (sprintf "getText: lang=%A, key=%A, isInitialized=%b" lang key isInitialized)
#endif

        if not isInitialized then
            // 初期化されていない場合はデフォルト値を使用
            let result = DefaultResources.getDefault lang key
#if FABLE_COMPILER
            Dom.console.debug (sprintf "Using default text: %s" result)
#endif
            result
        else if
            // 翻訳辞書から取得
            translations.ContainsKey(key) && translations.[key].ContainsKey(lang)
        then
            let result = translations.[key].[lang]
#if FABLE_COMPILER
            Dom.console.debug (sprintf "Using dictionary text: %s" result)
#endif
            result
        else
            // キーが見つからない場合はデフォルト値にフォールバック
            let fallback = DefaultResources.getDefault lang key
#if FABLE_COMPILER
            Dom.console.warn (sprintf "Missing translation: lang=%A, key=%A, using fallback: %s" lang key fallback)
#endif
            fallback

    /// 翻訳ファイルを読み込む関数
    let loadTranslations (csvContent: string) =
        try
            parseCsvContent csvContent
            true
        with ex ->
            // エラーが発生した場合はfalseを返す
#if FABLE_COMPILER
            Dom.console.error ("Failed to load translations: " + ex.Message)
#else
            Console.WriteLine("Failed to load translations: " + ex.Message)
#endif
            false

#if FABLE_COMPILER
    /// Fable用のCSVファイル読み込み関数（Http.get版）
    let loadTranslationsFromFile () =
        if not isInitialized then
            async {
                try
                    // Http.getを使用してCSVファイルを取得
                    let! (statusCode, responseText) = Http.get "/translations.csv"

                    if statusCode = 200 then
                        loadTranslations responseText |> ignore
                        Dom.console.debug (sprintf "Successed to load translations: Status code %d" statusCode)
                    else
                        Dom.console.error (sprintf "Failed to load translations: Status code %d" statusCode)
                with ex ->
                    Dom.console.error ("Failed to load translations file: " + ex.Message)
            }
            |> Async.StartImmediate
#endif
