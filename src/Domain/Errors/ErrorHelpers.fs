namespace Domain.Errors

open Common.Utilities

/// エラー操作のためのヘルパー関数
module ErrorHelpers =

    /// バリデーションエラーを作成
    let createValidationError field messageKey =
        { DomainError.Details = ValidationError(field, messageKey, Map.empty)
          ErrorContext = None }
        :> IError

    /// パラメータ付きバリデーションエラーを作成
    let createValidationErrorWithParams field messageKey parameters =
        { DomainError.Details = ValidationError(field, messageKey, parameters)
          ErrorContext = None }
        :> IError

    /// 見つからないエラーを作成
    let createNotFoundError entityType id =
        { DomainError.Details = NotFoundError(entityType, id)
          ErrorContext = None }
        :> IError

    /// ビジネスルール違反エラーを作成
    let createBusinessRuleError rule messageKey =
        { DomainError.Details = BusinessRuleViolation(rule, messageKey, Map.empty)
          ErrorContext = None }
        :> IError

    /// パラメータ付きビジネスルール違反エラーを作成
    let createBusinessRuleErrorWithParams rule messageKey parameters =
        { DomainError.Details = BusinessRuleViolation(rule, messageKey, parameters)
          ErrorContext = None }
        :> IError

    /// 成功結果を作成
    let asSuccess<'T> (value: 'T) : Result<'T> = Ok value

    /// 失敗結果を作成
    let asError<'T> (err: IError) : Result<'T> = Error err
    
    /// 条件に基づき成功または失敗を返す
    let resultIf condition errorFactory value =
        if condition then
            asSuccess value
        else
            asError (errorFactory())
            
    /// エラーにコンテキスト情報を追加
    let withContext (error: IError) (contextMap: Map<string, string>) : IError =
        error.WithContext contextMap