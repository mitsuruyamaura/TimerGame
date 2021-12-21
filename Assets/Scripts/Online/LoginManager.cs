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

    public static bool isSetup;

    /// <summary>
    /// 初期化処理
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static async UniTaskVoid InitializeAsync() {

        //Debug.Log("初期化開始");
        isSetup = false;

        // PlayFab へのログイン準備とログイン
        await PrepareLoginPlayPab();

        Debug.Log("初期化完了");

        isSetup = true;
    }

    /// <summary>
    /// PlayFab へのログイン準備とログイン
    /// </summary>
    public static async UniTask PrepareLoginPlayPab() {

        Debug.Log("ログイン 準備 開始");

        await LoginAndUpdateLocalCacheAsync();
    }

    /// <summary>
    /// ユーザーデータとタイトルデータを初期化
    /// </summary>
    /// <returns></returns>
    public static async UniTask LoginAndUpdateLocalCacheAsync() {

        Debug.Log("初期化開始");

        // ユーザーID の取得を試みる
        var userId = PlayerPrefsManager.UserId;

        // ユーザーID が取得できない場合には新規作成して匿名ログインする
        // 取得できた場合には、ユーザーID を使ってログインする
        var loginResult = string.IsNullOrEmpty(userId)
            ? await CreateNewUserAsync() : await LoadUserAsync(userId);

        // プレイヤーデータの作成と更新
        //await CreateUserDataAsync();

        // PlayFab のデータを自動で取得する設定にしているので、取得したデータをローカルにキャッシュする
        UpdateLocalCacheAsync(loginResult);
    }

    /// <summary>
    /// 新規ユーザーを作成して UserId を PlayerPrefs に保存
    /// </summary>
    /// <returns></returns>
    private static async UniTask<LoginResult> CreateNewUserAsync() {

        Debug.Log("ユーザーデータなし。新規ユーザー作成");

        while (true) {

            // UserId の採番
            var newUserId = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 20);

            // ログインリクエストの作成
            var request = new LoginWithCustomIDRequest {
                CustomId = newUserId,
                CreateAccount = true,
                InfoRequestParameters = CombinedInfoRequestParams
            };

            // PlayFab にログイン
            var response = await PlayFabClientAPI.LoginWithCustomIDAsync(request);

            // エラーがないか確認
            if (response.Error != null) {
                Debug.Log("Error");
            }

            // もしも LastLoginTime に値が入っている場合には、採番した ID が既存ユーザーと重複しているのでリトライする
            if (response.Result.LastLoginTime.HasValue) {
                continue;
            }

            // PlayerPrefs に UserId を記録する
            PlayerPrefsManager.UserId = newUserId;

            return response.Result;
        }
    }

    /// <summary>
    /// ログインしてユーザーデータをロード
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    private static async UniTask<LoginResult> LoadUserAsync(string userId) {

        Debug.Log("ユーザーデータあり。ログイン開始");

        // ログインリクエストの作成
        var request = new LoginWithCustomIDRequest {
            CustomId = userId,
            CreateAccount = false,
            InfoRequestParameters = CombinedInfoRequestParams
        };

        // PlayFab にログイン
        var response = await PlayFabClientAPI.LoginWithCustomIDAsync(request);

        // エラーハンドリング
        if (response.Error != null) {
            Debug.Log("Error");

            // TODO response.Error にはエラーの種類が値として入っている
            // そのエラーに対応した処理を switch 文などで記述して複数のエラーに対応できるようにする


        }

        // エラーの内容を見てハンドリングを行い、ログインに成功しているかを判定
        var message = response.Error is null ? $"Login success! My PlayFabID is {response.Result.PlayFabId}" : response.Error.GenerateErrorReport();

        Debug.Log(message);

        return response.Result;
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

    /// <summary>
    /// プレイヤーデータの作成と更新。デバッグ用
    /// </summary>
    private static async UniTask CreateUserDataAsync() {

        UserDataManager.User = User.Create();      //  新しく User 作成します
        string key = "User";　　　　　　　　　　　 //　保存用の Key を作成します

        await UserDataManager.UpdateUserDataByJsonAsync(key);   // PlayFab に Json 形式にした User クラスの情報を登録します

        Debug.Log("ユーザーデータ 登録完了");
    }

    /// <summary>
    /// PlayFab から取得したデータ群をローカル(端末)にキャッシュ
    /// </summary>
    /// <param name="loginResult"></param>
    /// <returns></returns>

    public static void UpdateLocalCacheAsync(LoginResult loginResult) {

        // TODO カタログ類の初期化。他のインスタンスの初期化にも必要なので最初に行う


        // タイトルデータの取得
        TitleDataManager.SyncPlayFabToClient(loginResult.InfoResultPayload.TitleData);


        // ユーザーデータの取得
        UserDataManager.SyncPlayFabToClient(loginResult.InfoResultPayload.UserData);

        // ユーザー名などの取得
        PlayerPlofileManager.SyncPlayFabToClient(loginResult.InfoResultPayload.PlayerProfile, loginResult.InfoResultPayload.PlayerStatistics);

        // TODO 他の初期化処理を追加


        Debug.Log("各種データのキャッシュ完了");
    }
}