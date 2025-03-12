namespace Domain.ValueObjects

open Domain.ValueObjects.Localization

/// ユーザー認証と言語設定に関するドメインモデル
module User =
    /// ログイン認証情報（入力値）
    type LoginCredentials =
        { Username: string
          Password: string
          Language: Language }

    /// ユーザープロファイル（ドメインエンティティ）
    /// ビジネスロジックに関連する属性のみを含む
    type UserProfile =
        { UserId: string // ユーザー識別子
          Username: string // 表示名
          Language: Language // 言語設定
          IsAuthenticated: bool // 認証状態
          LastLoginAt: System.DateTime option } // 最終ログイン日時
