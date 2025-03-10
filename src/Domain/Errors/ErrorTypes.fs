namespace Domain.Errors

/// エラーカテゴリーの定義
type ErrorCategory =
    | DomainError
    | UIError
    | InfrastructureError

/// 汎用エラー型 - obj型を使用
type Error =
    { Category: ErrorCategory
      Details: obj
      Context: Map<string, string> option }

/// ドメインエラーの詳細
type DomainErrorDetails =
    | ValidationError of field: string * message: string
    | NotFoundError of entityType: string * id: string
    | BusinessRuleViolation of rule: string * details: string

/// UIエラーの詳細
type UIErrorDetails =
    | MissingInput of fieldName: string
    | InvalidSelection of selection: string
    | FormError of message: string

/// インフラストラクチャエラーの詳細
type InfrastructureErrorDetails =
    | NetworkError of message: string
    | AuthenticationError of message: string
    | AuthorizationError of message: string
    | SystemError of message: string

/// エラーコンテキスト情報型
type ErrorContext =
    { Timestamp: System.DateTime
      RequestId: System.Guid option
      UserId: string option }

/// エラー結果型（Railway Oriented Programming用）
type Result<'T> =
    | Success of 'T
    | Failure of Error