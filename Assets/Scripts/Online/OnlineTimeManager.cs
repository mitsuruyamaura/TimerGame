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
}
