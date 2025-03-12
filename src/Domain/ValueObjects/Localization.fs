namespace Domain.ValueObjects

// 言語と地域化に関するドメインモデル
module Localization =
    // アプリケーションでサポートする言語
    type Language =
        | English
        | Japanese

        static member FromString(str: string) =
            match str.ToLower() with
            | "ja"
            | "japanese" -> Japanese
            | _ -> English

        static member ToString(lang: Language) =
            match lang with
            | English -> "en"
            | Japanese -> "ja"

        static member ToDisplayName(lang: Language) =
            match lang with
            | English -> "English"
            | Japanese -> "日本語"

        static member DefaultLocale(lang: Language) =
            match lang with
            | English -> "en-US"
            | Japanese -> "ja-JP"

        /// 日付をフォーマットする補助関数
        static member FormatDate (date: System.DateTime) (lang: Language) =
            // Fable互換の日付フォーマット (CultureInfoを使わない)
            let dateStr = date.ToString("yyyy-MM-dd")

            match lang with
            | English -> dateStr
            | Japanese -> dateStr.Replace("-", "/")

        /// 数値をフォーマットする補助関数
        static member FormatNumber (number: decimal) (lang: Language) =
            // Fable互換の数値フォーマット (CultureInfoを使わない)
            let numStr = number.ToString("0")

            match lang with
            | English -> numStr
            | Japanese -> numStr
