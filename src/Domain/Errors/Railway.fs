namespace Domain.Errors

/// Railway Oriented Programming のための演算子と関数
module Railway =
    // F#標準のResult関数を活用する

    /// 結果型にバインドする（標準Result.bindのラッパー）
    let bind (f: 'a -> Result<'b, IError>) (result: Result<'a, IError>) : Result<'b, IError> = Result.bind f result

    /// 値を関数に適用した結果を返す（標準Result.mapのラッパー）
    let map (f: 'a -> 'b) (result: Result<'a, IError>) : Result<'b, IError> = Result.map f result

    /// エラーをマップする（標準Result.mapErrorのラッパー）
    let mapError (f: IError -> IError) (result: Result<'a, IError>) : Result<'a, IError> = Result.mapError f result

    /// 結果をフォールドする（標準Result.foldのラッパー）
    let fold (successFunc: 'a -> 'b) (failureFunc: IError -> 'b) (result: Result<'a, IError>) : 'b =
        match result with
        | Ok value -> successFunc value
        | Error error -> failureFunc error

    /// 2つの関数を合成する（バインド演算子）
    let (>>=) result f = bind f result

    /// 関数適用演算子
    let (<!>) = map

    /// 逆方向の関数適用演算子
    let (<*>) resultF resultX =
        match resultF, resultX with
        | Ok f, Ok x -> Ok(f x)
        | Error e, _ -> Error e
        | _, Error e -> Error e

    /// エラーキャッチ演算子
    let (>>!) result errorHandler =
        match result with
        | Ok value -> Ok value
        | Error error -> errorHandler error

    /// 値が条件を満たすか検証し、満たさない場合はエラーを返す
    let validate predicate error x =
        if predicate x then Ok x else Error error

    /// 2つの結果を結合して2項組を作る
    let zip (result1: Result<'a, IError>) (result2: Result<'b, IError>) : Result<'a * 'b, IError> =
        match result1, result2 with
        | Ok v1, Ok v2 -> Ok(v1, v2)
        | Error e, _ -> Error e
        | _, Error e -> Error e

// F# 6.0以降のresultコンピュテーション式を活用
// 独自実装不要
