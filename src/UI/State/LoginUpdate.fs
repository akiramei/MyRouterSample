namespace UI.State

open Elmish
open UI.State.ViewModels
open UI.State.Messages
open Domain.ValueObjects.User
open Domain.Errors
open Application.ErrorTranslation
open Application.Services.ErrorMessageService

/// ログイン関連の状態更新
module LoginUpdate =
    /// ログインバリデーション
    let validateLogin (state: LoginPageState) : Result<LoginCredentials, IError> =
        // 入力が有効かをチェック
        if System.String.IsNullOrWhiteSpace state.Username then
            Error(ErrorHelpers.validation "username" "error.field.required")
        elif state.Username.Length < 3 then
            Error(
                ErrorHelpers.validationWithParams
                    "username"
                    "error.field.min.length"
                    (Map [ ("min", "3"); ("field", "Username") ])
            )
        elif System.String.IsNullOrWhiteSpace state.Password then
            Error(ErrorHelpers.validation "password" "error.field.required")
        elif state.Password.Length < 6 then
            Error(
                ErrorHelpers.validationWithParams
                    "password"
                    "error.field.min.length"
                    (Map [ ("min", "6"); ("field", "Password") ])
            )
        else
            // 検証に通過した場合は認証情報を返す
            Ok
                { Username = state.Username
                  Password = state.Password
                  Language = state.Language }

    /// ログイン認証
    let authenticateUser (credentials: LoginCredentials) : Result<UserProfile, IError> =
        // 実際の実装では、サーバーに認証をリクエストするなど
        // ここではダミーの実装
        if credentials.Username = "admin" && credentials.Password = "password" then
            Ok
                { UserId = "user-001"
                  Username = credentials.Username
                  Language = credentials.Language
                  IsAuthenticated = true
                  LastLoginAt = Some System.DateTime.Now }
        else
            Error(ErrorHelpers.businessRule "authentication" "error.authentication.failed")

    /// ログインページの状態更新
    let update (msg: LoginMsg) (state: LoginPageState) : LoginPageState * Cmd<LoginMsg> =
        match msg with
        | SetUsername username -> { state with Username = username }, Cmd.none

        | SetPassword password -> { state with Password = password }, Cmd.none

        | SetLanguage language -> { state with Language = language }, Cmd.none

        | LoginSubmit ->
            // 送信中の状態に変更
            let submittingState = { state with IsSubmitting = true }

            // 入力のバリデーション
            match validateLogin state with
            | Ok credentials ->
                // 認証処理
                match authenticateUser credentials with
                | Ok userProfile ->
                    { submittingState with
                        IsSubmitting = false
                        Error = None },
                    Cmd.ofMsg (LoginSuccess userProfile)

                | Error error ->
                    { submittingState with
                        IsSubmitting = false
                        Error = Some error },
                    Cmd.ofMsg (LoginFailed error)

            | Error error ->
                { submittingState with
                    IsSubmitting = false
                    Error = Some error },
                Cmd.ofMsg (LoginFailed error)

        | LoginSuccess _ ->
            // 成功時はエラー表示をクリア
            { state with
                IsSubmitting = false
                Error = None },
            Cmd.none

        | LoginFailed error ->
            // エラー情報を保持
            { state with
                IsSubmitting = false
                Error = Some error },
            Cmd.none
