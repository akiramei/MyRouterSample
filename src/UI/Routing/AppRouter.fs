namespace UI.Routing

open Feliz
open Feliz.Router
open Domain.ValueObjects.Types
open UI.State.Types
open UI.Components.Templates
open UI.Routing.PageRouter

/// アプリケーション全体のルーティングを担当するモジュール
/// URLの変更を検知し、適切なページとレイアウトを組み合わせる
module AppRouter =
    [<ReactComponent>]
    let AppRouter (model: Model) (dispatch: Msg -> unit) =
        // ページコンテンツを取得
        let pageContent = PageContent model dispatch
        // ルーターでURLの変更を監視
        React.router
            [ router.hashMode
              router.onUrlChanged (fun urlSegments -> dispatch (UrlChanged urlSegments))
              router.children
                  [
                    // 現在のページに基づいて適切なレイアウトを選択
                    match model.CurrentPage with
                    | Login ->
                        // 未認証レイアウト（ナビゲーションなし）
                        Layouts.UnauthenticatedLayout model dispatch pageContent
                    | _ ->
                        // 認証済みレイアウト（ナビゲーションあり）
                        Layouts.AuthenticatedLayout model dispatch pageContent ] ] 