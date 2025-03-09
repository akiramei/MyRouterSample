namespace Domain.Errors

/// Application error definitions
/// このモジュールは将来的なエラーハンドリングの拡張のために準備されています。
/// 現在は UI 層でのエラー処理に集中していますが、将来的にバックエンドとの連携時に
/// より堅牢なエラー処理を実装する際に活用される予定です。
module AppError =
    type AppError =
        | ValidationError of field: string * message: string
        | NotFoundError of entityType: string * id: string
        | BusinessRuleViolation of rule: string * details: string
        | AuthenticationError of message: string
        | AuthorizationError of message: string
        | SystemError of message: string
        
    /// エラーからエラーコードへの変換
    let toErrorCode (error: AppError) =
        match error with
        | ValidationError _ -> "ERR-VAL-001"
        | NotFoundError _ -> "ERR-NF-001"
        | BusinessRuleViolation _ -> "ERR-BIZ-001"
        | AuthenticationError _ -> "ERR-AUTH-001"
        | AuthorizationError _ -> "ERR-AUTH-002"
        | SystemError _ -> "ERR-SYS-001"