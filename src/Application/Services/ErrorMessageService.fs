namespace Application.Services

open Domain.Errors
open Domain.ValueObjects.Localization
open Shared.I18n.TranslationService

/// エラーメッセージサービス - エラーからユーザー向けメッセージへの変換を担当
module ErrorMessageService =
    /// エラーからメッセージキーを抽出
    let getMessageKey (error: IError) = error.MessageKey

    /// パラメータを置換してメッセージを整形
    let formatMessage (template: string) (parameters: Map<string, string>) =
        // パラメータの置換処理 - {field} などのプレースホルダを実際の値に置き換え
        parameters
        |> Map.fold (fun (message: string) key value -> message.Replace("{" + key + "}", value)) template

    /// エラーからユーザー向けメッセージを生成
    let getUserMessage (error: IError) (language: Language) =
        // 1. エラーのメッセージキーを使用して対応する翻訳テンプレートを取得
        let messageKey = getMessageKey error

        // リソースキーに変換（TranslationServiceで使用可能な形式に）
        let resourceKey =
            match messageKey with
            | "error.field.required" -> Some ResourceKey.UsernameRequired
            | "error.invalid.selection" -> Some ResourceKey.ValidationError
            | "error.entity.not.found" -> None // 追加のリソースキーがない場合
            | _ -> None

        // 2. リソースキーがある場合は翻訳サービスから取得、なければフォールバック
        let template =
            match resourceKey with
            | Some key -> getText language key
            | None ->
                // デフォルトメッセージ（英語/日本語）
                match language with
                | Language.English ->
                    match messageKey with
                    | "error.entity.not.found" -> "{entity} with ID {id} was not found"
                    | "error.network.connection" -> "Network error: {message}"
                    | "error.authentication.failed" -> "Authentication failed: {message}"
                    | "error.authorization.denied" -> "You don't have permission to {action} this {resource}"
                    | "error.system.generic" -> "System error occurred: {message}"
                    | _ -> "An error occurred"
                | Language.Japanese ->
                    match messageKey with
                    | "error.entity.not.found" -> "ID {id}の{entity}は見つかりませんでした"
                    | "error.network.connection" -> "ネットワークエラー: {message}"
                    | "error.authentication.failed" -> "認証エラー: {message}"
                    | "error.authorization.denied" -> "この{resource}を{action}する権限がありません"
                    | "error.system.generic" -> "システムエラーが発生しました: {message}"
                    | _ -> "エラーが発生しました"

        // 3. テンプレートにパラメータを適用
        formatMessage template error.MessageParams

    /// エラーのデバッグ情報を取得（開発者向け）
    let getDebugInfo (error: IError) =
        let errorInfo =
            sprintf "Error Code: %s, Message Key: %s" error.Code error.MessageKey

        let contextInfo =
            match error.Context with
            | Some context ->
                let contextStr =
                    context
                    |> Map.toList
                    |> List.map (fun (k, v) -> sprintf "%s: %s" k v)
                    |> String.concat ", "

                sprintf "Context: { %s }" contextStr
            | None -> "No context information"

        sprintf "%s\n%s" errorInfo contextInfo
