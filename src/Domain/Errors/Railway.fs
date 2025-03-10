namespace Domain.Errors

/// Railway Oriented Programming のための演算子と関数
module Railway =
    /// 結果型にバインドする
    let bind (f: 'a -> Result<'b>) (result: Result<'a>) : Result<'b> =
        match result with
        | Success value -> f value
        | Failure error -> Failure error
        
    /// 結果型にマップする
    let map (f: 'a -> 'b) (result: Result<'a>) : Result<'b> =
        match result with
        | Success value -> Success (f value)
        | Failure error -> Failure error
        
    /// エラーをマップする
    let mapError (f: Error -> Error) (result: Result<'a>) : Result<'a> =
        match result with
        | Success value -> Success value
        | Failure error -> Failure (f error)
        
    /// 結果をフォールドする
    let fold (successFunc: 'a -> 'b) (failureFunc: Error -> 'b) (result: Result<'a>) : 'b =
        match result with
        | Success value -> successFunc value
        | Failure error -> failureFunc error
        
    /// 2つの関数を合成する
    let (>>=) result f = bind f result
    
    /// 関数適用演算子
    let (<!>) = map
    
    /// 逆方向の関数適用演算子
    let (<*>) resultF resultX =
        match resultF, resultX with
        | Success f, Success x -> Success (f x)
        | Failure e, _ -> Failure e
        | _, Failure e -> Failure e
    
    /// エラーキャッチ演算子
    let (>>!) result errorHandler =
        match result with
        | Success value -> Success value
        | Failure error -> errorHandler error
        
    /// 値が条件を満たすか検証し、満たさない場合はエラーを返す
    let validate predicate error x =
        if predicate x then Success x
        else ErrorHelpers.toResult error
        
    /// 2つの結果を結合して2項組を作る
    let zip (result1: Result<'a>) (result2: Result<'b>) : Result<'a * 'b> =
        match result1, result2 with
        | Success v1, Success v2 -> Success (v1, v2)
        | Failure e, _ -> Failure e
        | _, Failure e -> Failure e