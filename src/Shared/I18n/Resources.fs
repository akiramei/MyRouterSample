namespace Shared.I18n

open Domain.ValueObjects.User

/// Application text resources for internationalization
module Resources =
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
        
    let getText (lang: Language) (key: ResourceKey) : string =
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