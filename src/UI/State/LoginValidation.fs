namespace UI.State

open Domain.Errors
open UI.State.Types

/// ログイン関連のバリデーション
module LoginValidation =
    /// ユーザー名のバリデーション
    let validateUsername (username: string) =
        if System.String.IsNullOrWhiteSpace username then
            Error(ErrorHelpers.validation "username" "error.field.required")
        elif username.Length < 3 then
            Error(ErrorHelpers.validationWithParams "username" "error.field.min.length" (Map [ ("min", "3") ]))
        else
            Ok username

    /// パスワードのバリデーション
    let validatePassword (password: string) =
        if System.String.IsNullOrWhiteSpace password then
            Error(ErrorHelpers.validation "password" "error.field.required")
        elif password.Length < 6 then
            Error(ErrorHelpers.validationWithParams "password" "error.field.min.length" (Map [ ("min", "6") ]))
        else
            Ok password

    /// ログインフォーム全体のバリデーション
    let validateLogin (model: LoginModel) =
        // F#のresultコンピュテーション式を使用
        result {
            let! validUsername = validateUsername model.Username
            let! validPassword = validatePassword model.Password

            return
                { Username = validUsername
                  Password = validPassword
                  Language = model.Language }
        }
