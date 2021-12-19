using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System.Threading.Tasks;
using System;
using Cysharp.Threading.Tasks;

public static class LoginManager {　　//　ゲーム実行時にインスタンスが自動的に１つだけ生成される

    /// <summary>
    /// ログインと同時に PlayFab から取得する情報の設定用クラスである GetPlayerCombinedInfoRequestParams のプロパティ。
    /// GetPlayerCombinedInfoRequestParams クラスで設定した値が InfoRequestParameters の設定値になり、true にしてある項目で各情報が自動的に取得できるようになる
    /// 各パラメータの初期値はすべて false
    /// 取得が多くなるほどログイン時間がかかり、メモリを消費するので気を付ける
    /// 取得結果は InfoResultPayLoad に入っている。false のものはすべて null になる
    /// </summary>
    public static GetPlayerCombinedInfoRequestParams CombinedInfoRequestParams { get; }
        = new GetPlayerCombinedInfoRequestParams {
            GetUserAccountInfo = true,
            GetPlayerProfile = true,
            GetTitleData = true,
            GetUserData = true,
            GetUserInventory = true,
            GetUserVirtualCurrency = true,
            GetPlayerStatistics = true
        };

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

    /// <summary>
    /// Email とパスワードでログイン(アカウント回復用)
    /// </summary>
    /// <param name="email"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public static async UniTask<(bool, string)> LoginEmailAndPasswordAsync(string email, string password) {

        // Email によるログインリクエストの作成
        var request = new LoginWithEmailAddressRequest {
            Email = email,
            Password = password,
            InfoRequestParameters = CombinedInfoRequestParams
        };

        // PlayFab にログイン
        var response = await PlayFabClientAPI.LoginWithEmailAddressAsync(request);

        // エラーハンドリング
        if (response.Error != null) {
            switch (response.Error.Error) {
                case PlayFabErrorCode.InvalidParams:
                case PlayFabErrorCode.InvalidEmailOrPassword:
                case PlayFabErrorCode.AccountNotFound:
                    Debug.Log("メールアドレスかパスワードが正しくありません");
                    break;
                default:
                    Debug.Log(response.Error.GenerateErrorReport());
                    break;
            }

            return (false, "メールアドレスかパスワードが正しくありません");
        }

        // PlayerPrefas を初期化して、ログイン結果の UserId を登録し直す
        PlayerPrefs.DeleteAll();

        // 新しく PlayFab から UserId を取得
        // InfoResultPayload はクライアントプロフィールオプション(InfoRequestParameters)で許可されてないと null になる
        PlayerPrefsManager.UserId = response.Result.InfoResultPayload.AccountInfo.CustomIdInfo.CustomId;

        // Email でログインしたことを記録する
        PlayerPrefsManager.IsLoginEmailAdress = true;

        return (true, "Email によるログインが完了しました。");
    }
}