namespace UI.Routing

open Feliz
open UI.State
open UI.State.ViewModels
open UI.State.Messages
open UI.Pages.HomePage
open UI.Pages.CounterPage
open UI.Pages.UserProfilePage
open UI.Pages.NotFoundPage
open UI.Pages.LoginPage

/// ページのルーティングを担当するモジュール
/// ページコンテンツの選択のみを担当し、レイアウトには関与しない
module PageRouter =
    [<ReactComponent>]
    let PageContent (state: ApplicationState) (dispatch: AppMsg -> unit) =
        // 現在のページに基づいて適切なページコンポーネントを返す
        match state.CurrentPage with
        | Login -> LoginPage state.LoginPage (LoginMsg >> dispatch)

        | Home -> HomePage()

        | Counter -> CounterPage state.CounterPage (CounterMsg >> dispatch)

        | UserProfile userId ->
            // ユーザープロファイルページの表示
            // ユーザーIDが変わった場合のみデータを読み込む
            if state.UserProfilePage.UserId <> Some userId then
                dispatch (UserProfileMsg(LoadUserData userId))

            UserProfilePage userId state.UserProfilePage.IsLoading

        | NotFound -> NotFoundPage()
