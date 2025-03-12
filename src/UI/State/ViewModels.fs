namespace UI.State

open Domain.ValueObjects
open Domain.ValueObjects.Localization
open Domain.Errors

type Page =
    | Login
    | Home
    | Counter
    | UserProfile of string
    | NotFound

/// UI層の状態を表すビューモデル
/// UIの見た目や操作に関する技術的な関心事のみを含む
module ViewModels =
    /// ホームページの状態
    type HomePageState = { IsLoading: bool }

    /// カウンターページの状態
    type CounterPageState =
        { Count: int
          IsIncrementing: bool
          IsDecrementing: bool }

    /// ユーザープロファイルページの状態
    type UserProfilePageState =
        { UserId: string option
          Username: string
          IsLoading: bool
          IsEditing: bool
          ValidationErrors: string list }

    /// ログインページの状態
    type LoginPageState =
        { Username: string
          Password: string
          Language: Language
          IsSubmitting: bool
          Error: IError option }

    /// エラー表示の状態
    type ErrorDisplayState =
        { IsVisible: bool
          Message: string option }

    /// アプリケーション全体の状態
    type ApplicationState =
        { CurrentUser: User.UserProfile option
          CurrentUrl: string list
          CurrentPage: Page
          HomePage: HomePageState
          CounterPage: CounterPageState
          UserProfilePage: UserProfilePageState
          LoginPage: LoginPageState
          ErrorDisplay: ErrorDisplayState }
