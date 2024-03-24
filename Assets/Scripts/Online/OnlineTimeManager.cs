using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using UnityEngine;

public class OnlineTimeManager : MonoBehaviour
{
    private const string FORMAT = "yyyy/MM/dd HH:mm:ss";

    //private async UniTaskVoid Start() {

    //    // UniTask のみの場合(現在は UniTask + UniRx にて実装中)
    //    // キャンセレーショントークンの作成
    //    var ct = this.GetCancellationTokenOnDestroy();

    //    // OnApplicationQuit メソッドを非同期に設定する
    //    await this.GetAsyncApplicationQuitTrigger().OnApplicationQuitAsync(ct);
    //}

    // Zenn 用のサンプル　UniTask + UniRx での OnApplocationQuit メソッドの非同期処理の待機処理
    //private static async UniTask QuitGameAsync() {

    //    await UpdateLogOffTimeAsync();

    //    Debug.Log("セーブ完了①。");

    //    await UniTask.WhenAll(
    //        UpdateLogOffTimeAsync(),
    //        UpdateLogOffTimeAsync(),
    //        UpdateLogOffTimeAsync(),
    //        UpdateLogOffTimeAsync()
    //        );
    //    Debug.Log("セーブ完了②。");
    //    Debug.Log("ゲームを終了します。");
    //}

    /// <summary>
    /// OnApplicationQuit 時に実行するメソッド。今回はログオフ時間をサーバーにセーブ
    /// </summary>
    /// <returns></returns>
    public static async UniTask<bool> UpdateLogOffTimeAsync() {

        string dateTimeString = DateTime.Now.ToString(FORMAT);

        var request = new UpdateUserDataRequest {

            Data = new Dictionary<string, string> { { "LogOffTime", dateTimeString } }
        };

        var response = await PlayFabClientAPI.UpdateUserDataAsync(request);

        if (response.Error!= null) {
            Debug.Log("エラー");
            return false;
        }

        Debug.Log("サーバーにログオフ時の時刻セーブ完了");
        return true;
    }

    /// <summary>
    /// お使いの時間のセーブ
    /// お使い開始時とゲーム終了時にセーブ
    /// </summary>
    /// <param name="workingJobTimeDatasList"></param>
    /// <returns></returns>
    public static async UniTask UpdateJobTimeAsync(List<OfflineTimeManager.JobTimeData> workingJobTimeDatasList) {

        string json = JsonConvert.SerializeObject(workingJobTimeDatasList);

        var request = new UpdateUserDataRequest {

            Data = new Dictionary<string, string> { { "JobTimes", json } }
        };

        var response = await PlayFabClientAPI.UpdateUserDataAsync(request);

        if (response.Error != null) {
            Debug.Log("エラー");
            return;
        }

        Debug.Log("サーバーに仕事のデータ セーブ完了");
    }
}
