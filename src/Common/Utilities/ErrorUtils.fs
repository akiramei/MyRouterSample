namespace Common.Utilities

/// エラー処理に関する共通ユーティリティ
module ErrorUtils =
    /// コンテキストマップを既存のコンテキストにマージする共通実装
    let mergeContexts (existingContext: Map<string, string> option) (newContext: Map<string, string>) : Map<string, string> option =
        match existingContext with
        | Some existing -> 
            Some(Map.fold (fun acc k v -> Map.add k v acc) existing newContext)
        | None -> 
            Some newContext
            
    /// 既存のコンテキストとマップをマージし、新しいエラーを作成する関数を適用
    let withContext (error: 'TError) 
                    (contextGetter: 'TError -> Map<string, string> option) 
                    (contextSetter: 'TError -> Map<string, string> option -> 'TError) 
                    (newContext: Map<string, string>) : 'TError =
        let existingContext = contextGetter error
        let mergedContext = mergeContexts existingContext newContext
        contextSetter error mergedContext