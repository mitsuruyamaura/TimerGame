using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using Cysharp.Threading.Tasks.Linq;
using System;
using System.Linq;
using UniRx;
using PlayFab.ClientModels;
using PlayFab;
using Newtonsoft.Json;

public class OnlineTimeManager : MonoBehaviour
{
    private const string FORMAT = "yyyy/MM/dd HH:mm:ss";

    private async UniTaskVoid Start() {

        var ct = this.GetCancellationTokenOnDestroy();

        await this.GetAsyncApplicationQuitTrigger().OnApplicationQuitAsync(ct);
    }

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

        Debug.Log("ログオフ時の時刻セーブ完了");
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

        Debug.Log("仕事のデータ セーブ完了");
    }
}
