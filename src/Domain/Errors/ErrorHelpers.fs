namespace Domain.Errors

/// エラー操作のためのヘルパー関数
module ErrorHelpers =
    
    /// ドメインエラーをラップするヘルパー関数
    let asDomainError (details: DomainErrorDetails) context =
        { Category = DomainError
          Details = details :> obj
          Context = context }
    
    /// UIエラーをラップするヘルパー関数
    let asUIError (details: UIErrorDetails) context =
        { Category = UIError
          Details = details :> obj
          Context = context }
          
    /// インフラストラクチャエラーをラップするヘルパー関数
    let asInfrastructureError (details: InfrastructureErrorDetails) context =
        { Category = InfrastructureError
          Details = details :> obj
          Context = context }
          
    /// エラーコードへの変換
    let toErrorCode (error: Error) =
        match error.Category with
        | DomainError ->
            match error.Details with
            | :? DomainErrorDetails as details ->
                match details with
                | ValidationError _ -> "ERR-VAL-001"
                | NotFoundError _ -> "ERR-NF-001" 
                | BusinessRuleViolation _ -> "ERR-BIZ-001"
            | _ -> "ERR-DOM-000"
        | UIError ->
            match error.Details with
            | :? UIErrorDetails as details ->
                match details with
                | MissingInput _ -> "ERR-UI-001"
                | InvalidSelection _ -> "ERR-UI-002"
                | FormError _ -> "ERR-UI-003"
            | _ -> "ERR-UI-000"
        | InfrastructureError ->
            match error.Details with
            | :? InfrastructureErrorDetails as details ->
                match details with
                | NetworkError _ -> "ERR-INF-001"
                | AuthenticationError _ -> "ERR-INF-002"
                | AuthorizationError _ -> "ERR-INF-003" 
                | SystemError _ -> "ERR-INF-004"
            | _ -> "ERR-INF-000"
    
    /// ユーザー向けエラーメッセージの取得
    let toUserMessage (error: Error) =
        match error.Category with
        | DomainError ->
            match error.Details with
            | :? DomainErrorDetails as details ->
                match details with
                | ValidationError (field, message) -> sprintf "入力エラー: %s - %s" field message
                | NotFoundError (entity, id) -> sprintf "%s (ID: %s) が見つかりません" entity id
                | BusinessRuleViolation (rule, details) -> sprintf "ビジネスルール違反: %s - %s" rule details
            | _ -> "ドメインエラーが発生しました"
        | UIError ->
            match error.Details with
            | :? UIErrorDetails as details ->
                match details with
                | MissingInput field -> sprintf "%s は必須です" field
                | InvalidSelection selection -> sprintf "選択された値 '%s' は無効です" selection
                | FormError message -> sprintf "フォームエラー: %s" message
            | _ -> "UI操作エラーが発生しました"
        | InfrastructureError ->
            match error.Details with
            | :? InfrastructureErrorDetails as details ->
                match details with
                | NetworkError message -> sprintf "ネットワークエラー: %s" message
                | AuthenticationError message -> sprintf "認証エラー: %s" message
                | AuthorizationError message -> sprintf "権限エラー: %s" message
                | SystemError message -> sprintf "システムエラー: %s" message
            | _ -> "システムエラーが発生しました"
    
    /// Railway Oriented Programming用の結果型変換ヘルパー
    let toResult<'T> (error: Error) : Result<'T> =
        Failure error
        
    /// 成功結果の作成
    let success<'T> (value: 'T) : Result<'T> =
        Success value