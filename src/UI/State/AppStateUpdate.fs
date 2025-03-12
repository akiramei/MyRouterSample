namespace UI.State

open Elmish
open UI.State.ViewModels
open UI.State.Messages
open UI.Services.RouteService
open Domain.ValueObjects.User
open Application.Services.ErrorMessageService

/// アプリケーション全体の状態更新
module AppStateUpdate =

    /// カウンターページの状態更新
    let updateCounter (msg: CounterMsg) (state: CounterPageState) : CounterPageState * Cmd<CounterMsg> =
        match msg with
        | Increment ->
            { state with
                Count = state.Count + 1
                IsIncrementing = true },
            Cmd.none

        | Decrement ->
            { state with
                Count = state.Count - 1
                IsDecrementing = true },
            Cmd.none

        | Reset -> { state with Count = 0 }, Cmd.none

    /// ユーザープロファイルページの状態更新
    let updateUserProfile
        (msg: UserProfileMsg)
        (state: UserProfilePageState)
        (language: Language)
        : UserProfilePageState * Cmd<UserProfileMsg> =
        match msg with
        | LoadUserData userId ->
            { state with
                UserId = Some userId
                IsLoading = true },
            // 実際の実装ではユーザーデータの読み込みコマンドを返す
            Cmd.ofMsg (
                UserDataLoaded
                    { UserId = userId
                      Username = "User " + userId
                      Language = language
                      IsAuthenticated = true
                      LastLoginAt = Some System.DateTime.Now }
            )

        | UserDataLoaded profile ->
            { state with
                IsLoading = false
                Username = profile.Username },
            Cmd.none

        | UserDataError error ->
            { state with IsLoading = false },
            // エラーメッセージを作成
            Cmd.ofMsg (ProfileError error)

        | StartEditing -> { state with IsEditing = true }, Cmd.none

        | CancelEditing -> { state with IsEditing = false }, Cmd.none

        | SaveProfile ->
            // 実際の実装ではプロファイル保存の処理を行う
            state, Cmd.ofMsg ProfileSaved

        | ProfileSaved -> { state with IsEditing = false }, Cmd.none

        | ProfileError _ ->
            // エラー処理はAppStateで行うので、ここでは状態変更のみ
            state, Cmd.none

    /// アプリケーション全体の状態更新
    let update (msg: AppMsg) (state: ApplicationState) : ApplicationState * Cmd<AppMsg> =
        match msg with
        | LoginMsg loginMsg ->
            match loginMsg with
            | LoginSuccess userProfile ->
                // ログイン成功時はユーザー情報を保存し、ホームページに遷移
                let loginState, loginCmd = LoginUpdate.update loginMsg state.LoginPage

                { state with
                    LoginPage = loginState
                    CurrentUser = Some userProfile
                    CurrentPage = Home
                    // エラー表示をクリア
                    ErrorDisplay = { IsVisible = false; Message = None } },
                Cmd.batch [ Cmd.map LoginMsg loginCmd; navigateCmd "home" ]

            | LoginFailed error ->
                // ログイン失敗時はエラーを表示
                let loginState, loginCmd = LoginUpdate.update loginMsg state.LoginPage

                let errorMessage = getUserMessage error (state.LoginPage.Language)

                { state with
                    LoginPage = loginState
                    ErrorDisplay =
                        { IsVisible = true
                          Message = Some errorMessage } },
                Cmd.map LoginMsg loginCmd

            | _ ->
                // その他のログインメッセージはLoginUpdateに委譲
                let loginState, loginCmd = LoginUpdate.update loginMsg state.LoginPage
                { state with LoginPage = loginState }, Cmd.map LoginMsg loginCmd

        | CounterMsg counterMsg ->
            // カウンターページの更新
            match state.CurrentUser with
            | None ->
                // 未認証の場合はログインページにリダイレクト
                state, navigateCmd ""
            | Some _ ->
                let counterState, counterCmd = updateCounter counterMsg state.CounterPage

                { state with
                    CounterPage = counterState },
                Cmd.map CounterMsg counterCmd

        | UserProfileMsg profileMsg ->
            // ユーザープロファイルページの更新
            match state.CurrentUser with
            | None ->
                // 未認証の場合はログインページにリダイレクト
                state, navigateCmd ""
            | Some user ->
                let profileState, profileCmd =
                    updateUserProfile profileMsg state.UserProfilePage user.Language

                // ユーザープロファイルのエラー処理
                let (newState, globalCmd) =
                    match profileMsg with
                    | ProfileError error ->
                        let errorMessage = getUserMessage error user.Language

                        { state with
                            ErrorDisplay =
                                { IsVisible = true
                                  Message = Some errorMessage } },
                        Cmd.ofMsg (ShowError errorMessage)
                    | _ -> state, Cmd.none

                { newState with
                    UserProfilePage = profileState },
                Cmd.batch [ Cmd.map UserProfileMsg profileCmd; globalCmd ]

        | UrlChanged newUrl ->
            let newPage = parseUrl newUrl

            // ページ遷移時の認証チェック
            match newPage, state.CurrentUser with
            | Login, _ ->
                // ログインページは誰でもアクセス可能
                { state with
                    CurrentUrl = newUrl
                    CurrentPage = newPage },
                Cmd.none

            | _, None ->
                // 未認証の場合はログインページにリダイレクト
                { state with
                    CurrentUrl = []
                    CurrentPage = Login },
                navigateCmd ""

            | _, Some _ ->
                // 認証済みの場合は要求されたページに遷移
                { state with
                    CurrentUrl = newUrl
                    CurrentPage = newPage },
                Cmd.none

        | Logout ->
            // ログアウト処理
            { state with
                CurrentUser = None
                CurrentPage = Login
                LoginPage = AppState.initLoginPage () },
            navigateCmd ""

        | ShowError message ->
            // エラーメッセージの表示
            { state with
                ErrorDisplay =
                    { IsVisible = true
                      Message = Some message } },
            Cmd.none

        | ClearError ->
            // エラーメッセージのクリア
            { state with
                ErrorDisplay = { IsVisible = false; Message = None } },
            Cmd.none

        | HomeMsg _ ->
            // ホームページのメッセージ処理（現在は何もしない）
            state, Cmd.none
