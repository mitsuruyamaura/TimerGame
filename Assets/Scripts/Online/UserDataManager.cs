using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using PlayFab;
using PlayFab.ClientModels;
using Newtonsoft.Json;
using System;


public static class UserDataManager {

    // TODO Level などの情報を持たせる

    public static User User { get; set; }

    private const string FORMAT = "yyyy/MM/dd HH:mm:ss";

    public static Reward Reward { get; set; }



    /// <summary>
    /// プレイヤーデータ内の作成と更新(プレイヤーデータ(タイトル)の Key に１つだけ値を登録する方法)
    /// </summary>
    /// <param name="updateUserData"></param>
    /// <param name="userDataPermission"></param>
    public static async UniTask UpdatePlayerDataAsync(Dictionary<string, string> updateUserData, UserDataPermission userDataPermission = UserDataPermission.Private) {

        var request = new UpdateUserDataRequest {
            Data = updateUserData,

            // アクセス許可の変更
            Permission = userDataPermission
        };

        var response = await PlayFabClientAPI.UpdateUserDataAsync(request);

        if (response.Error != null) {

            Debug.Log("エラー");
            return;
        }

        Debug.Log("プレイヤーデータ　更新");
    }

    /// <summary>
    /// プレイヤーデータから指定した Key の情報の削除
    /// </summary>
    /// <param name="deleteKey">削除する Key の名前</param>
    public static async void DeletePlayerDataAsync(string deleteKey) {

        var request = new UpdateUserDataRequest {
            KeysToRemove = new List<string> { deleteKey }
        };

        var response = await PlayFabClientAPI.UpdateUserDataAsync(request);

        if (response.Error != null) {

            Debug.Log("エラー");
            return;
        }

        Debug.Log("プレイヤーデータ　削除");
    }

    /// <summary>
    /// プレイヤーデータの作成と更新(プレイヤーデータ(タイトル)の Key に複数の値情報をまとめた Json を利用する場合)
    /// </summary>
    /// <param name="userName">key</param>
    /// <param name="userDataPermission"></param>
    /// <returns></returns>
    public static async UniTask<(bool isSuccess, string errorMessage)> UpdateUserDataByJsonAsync(string userName, UserDataPermission userDataPermission = UserDataPermission.Private) {

        string userJson = JsonConvert.SerializeObject(User);　　　//　<=　この機能が Json.NET ライブラリの処理です。

        var request = new UpdateUserDataRequest {
            Data = new Dictionary<string, string> {
                { userName, userJson }
            },

            // アクセス許可の変更
            Permission = userDataPermission
        };

        var response = await PlayFabClientAPI.UpdateUserDataAsync(request);

        if (response.Error != null) {

            Debug.Log("エラー");
            return (false, response.Error.ToString());
        }

        return (true, string.Empty);
    }

    /// <summary>
    /// PlayFab の最新データを取得してローカルにキャッシュ
    /// </summary>
    /// <param name="userData"></param>
    public static void SyncPlayFabToClient(Dictionary<string, UserDataRecord> userData) {

        // ユーザーの情報を取得。取得できた場合には複合化、取得できない場合には新規ユーザーの作成
        User = userData.TryGetValue("User", out var user)
            ? JsonConvert.DeserializeObject<User>(user.Value) : User.Create();

        Debug.Log("PlayFab のユーザーデータを取得");

        // ログオフした時間の取得
        if (userData.TryGetValue("LogOffTime", out var logOffTime)) {

            OfflineTimeManager.instance.LoadDateTime = DateTime.ParseExact(logOffTime.Value, FORMAT, null);
            Debug.Log($"前回ログオフした時間 : { logOffTime.Value }");
        }

        // 仕事の情報を取得
        if (userData.TryGetValue("JobTimes", out var jobTimes)) {
            OfflineTimeManager.instance.workingJobTimeDatasList = JsonConvert.DeserializeObject<List<OfflineTimeManager.JobTimeData>>(jobTimes.Value);
        } else {
            Debug.Log("仕事のデータなし");
        }

        Debug.Log("PlayFab の仕事のデータを取得");

        // オンラインで経過した時間を計算
        //OfflineTimeManager.instance.CalculateOfflineDateTimeElasped(OfflineTimeManager.instance.LoadDateTime);


        // TODO 他にも処理があれば追加

    }

    /// <summary>
    /// 獲得した褒賞の追加・加算
    /// </summary>
    /// <param name="rewardData"></param>
    /// <param name="addCount"></param>
    public static void AddReward(RewardData rewardData, int addCount = 1) {

        // Reward がインスタンスされていない場合には初期化
        if(Reward == null) {
            Reward = Reward.Create();
        }

        Reward.rewardPoint += rewardData.rewardPoint;

        if (Reward.rewardInfosList.Exists(x => x.rewardNo == rewardData.rewardNo)) {
            Reward.rewardInfosList.Find(x => x.rewardNo == rewardData.rewardNo).rewardCount++;
        } else {
            Reward.rewardInfosList.Add(new RewardInfo { rewardNo = rewardData.rewardNo, rewardCount = addCount });
        }
    }


    /// <summary>
    /// 獲得した褒賞のデータの更新
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="userDataPermission"></param>
    /// <returns></returns>
    public static async UniTask<(bool isSuccess, string errorMessage)> UpdateHaveRewardDataAsync(string userName, UserDataPermission userDataPermission = UserDataPermission.Private) {

        string rewardJson = JsonConvert.SerializeObject(Reward);

        var request = new UpdateUserDataRequest {
            Data = new Dictionary<string, string> { { userName, rewardJson } },

            Permission = userDataPermission
        };

        var response = await PlayFabClientAPI.UpdateUserDataAsync(request);

        if (response.Error != null) {
            Debug.Log("エラー");
            return (false, response.Error.ToString());
        }

        return (true, string.Empty);
    }



}