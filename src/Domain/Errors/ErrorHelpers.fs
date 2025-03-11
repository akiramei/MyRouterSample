namespace Domain.Errors

/// エラー操作のためのヘルパー関数
module ErrorHelpers =

    /// バリデーションエラーを作成
    let validation field messageKey =
        { DomainError.Details = ValidationError(field, messageKey, Map.empty)
          ErrorContext = None }
        :> IError

    /// パラメータ付きバリデーションエラーを作成
    let validationWithParams field messageKey parameters =
        { DomainError.Details = ValidationError(field, messageKey, parameters)
          ErrorContext = None }
        :> IError

    /// 見つからないエラーを作成
    let notFound entityType id =
        { DomainError.Details = NotFoundError(entityType, id)
          ErrorContext = None }
        :> IError

    /// ビジネスルール違反エラーを作成
    let businessRule rule messageKey =
        { DomainError.Details = BusinessRuleViolation(rule, messageKey, Map.empty)
          ErrorContext = None }
        :> IError

    /// パラメータ付きビジネスルール違反エラーを作成
    let businessRuleWithParams rule messageKey parameters =
        { DomainError.Details = BusinessRuleViolation(rule, messageKey, parameters)
          ErrorContext = None }
        :> IError

    /// 成功結果を作成
    let success<'T> (value: 'T) : Result<'T> = Ok value

    /// 失敗結果を作成
    let error<'T> (err: IError) : Result<'T> = Error err
