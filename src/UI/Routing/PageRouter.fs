namespace UI.Routing

open Feliz
open Domain.ValueObjects.Types
open UI.State.Types
open UI.Pages.HomePage
open UI.Pages.CounterPage
open UI.Pages.UserProfilePage
open UI.Pages.NotFoundPage
open UI.Pages.LoginPage

/// ページのルーティングを担当するモジュール
/// ページコンテンツの選択のみを担当し、レイアウトには関与しない
module PageRouter =
    [<ReactComponent>]
    let PageContent (model: Model) (dispatch: Msg -> unit) =
        // プロファイル読み込み処理をuseCallbackでメモ化
        let checkAndLoadProfile =
            React.useCallback (
                (fun username ->
                    if model.UserProfile.Username <> username then
                        dispatch (UserProfileMsg(LoadUserData username))),
                [| model.UserProfile.Username :> obj |]
            )

        // 現在のページに基づいて適切なページコンポーネントを返す
        match model.CurrentPage with
        | Login -> LoginPage model.Login (fun msg -> dispatch (LoginMsg msg))
        | Home -> HomePage()
        | Counter -> CounterPage model.Counter (fun msg -> dispatch (CounterMsg msg))
        | UserProfile username ->
            checkAndLoadProfile (username)
            UserProfilePage username model.UserProfile.IsLoading
        | NotFound -> NotFoundPage()
