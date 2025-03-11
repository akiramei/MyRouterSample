namespace Domain.Errors

/// エラー操作のためのヘルパー関数
module ErrorHelpers =

    /// ドメインエラーを作成するヘルパー関数
    let asDomainError (details: DomainErrorDetails) context : IError =
        { DomainError.Details = details
          ErrorContext = context }
        :> IError

    /// 成功結果の作成
    let success<'T> (value: 'T) : Result<'T> = Success value

    /// 失敗結果の作成
    let failure (error: IError) : Result<'T> = Failure error
