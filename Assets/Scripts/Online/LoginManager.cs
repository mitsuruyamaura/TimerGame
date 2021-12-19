using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System.Threading.Tasks;
using System;
using Cysharp.Threading.Tasks;

public static class LoginManager {　　//　ゲーム実行時にインスタンスが自動的に１つだけ生成される

    /// <summary>
    /// コンストラクタ
    /// </summary>
    static LoginManager() {

        // TitleId 設定
        PlayFabSettings.staticSettings.TitleId = "457D7";

        Debug.Log("TitleID 設定: " + PlayFabSettings.staticSettings.TitleId);
    }

    /// <summary>
    /// 初期化処理
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static async UniTaskVoid InitializeAsync() {

        Debug.Log("初期化開始");

        // PlayFab へのログイン準備とログイン
        await PrepareLoginPlayPab();

        Debug.Log("初期化完了");
    }

    /// <summary>
    /// PlayFab へのログイン準備とログイン
    /// </summary>
    public static async UniTask PrepareLoginPlayPab() {

        Debug.Log("ログイン 準備 開始");

        // 仮のログインの情報(リクエスト)を作成して設定
        var request = new LoginWithCustomIDRequest {
            CustomId = "GettingStartedGuide",　　　　　// この部分がユーザーのIDになります
            CreateAccount = true                       // アカウントが作成されていない場合、true の場合は匿名ログインしてアカウントを作成する
        };

        // PlayFab へログイン。情報が確認できるまで待機
        var result = await PlayFabClientAPI.LoginWithCustomIDAsync(request);

        // エラーの内容を見て、ログインに成功しているかを判定(エラーハンドリング)
        var message = result.Error is null
            ? $"ログイン成功! My PlayFabID is { result.Result.PlayFabId }"    // Error が null ならば[エラーなし]なので、ログイン成功
            : result.Error.GenerateErrorReport();                             // Error が null 以外の場合はエラーが発生しているので、レポート作成

        Debug.Log(message);
    }
}