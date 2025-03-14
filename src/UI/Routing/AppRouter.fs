namespace UI.Routing

open Feliz
open Feliz.Router
open UI.State
open UI.State.ViewModels
open UI.State.Messages
open UI.Components.Templates.Layouts
open UI.Routing.PageRouter

/// アプリケーション全体のルーティングを担当するモジュール
/// URLの変更を検知し、適切なページとレイアウトを組み合わせる
module AppRouter =
    let private urlChangedHandler dispatch urlSegments = dispatch (UrlChanged urlSegments)

    let AppRouter (state: ApplicationState) (dispatch: AppMsg -> unit) =
        // ページコンテンツを取得
        let pageContent = PageContent state dispatch

        // ルーターでURLの変更を監視
        React.router
            [ router.hashMode
              router.onUrlChanged (urlChangedHandler dispatch)
              router.children
                  [
                    // 現在のページに基づいて適切なレイアウトを選択
                    match state.CurrentPage with
                    | Login ->
                        // 未認証レイアウト（ナビゲーションなし）
                        UnauthenticatedLayout state dispatch pageContent
                    | _ ->
                        // 認証済みレイアウト（ナビゲーションあり）
                        AuthenticatedLayout state dispatch pageContent ] ]
