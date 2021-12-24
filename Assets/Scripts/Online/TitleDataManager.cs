using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.Linq;


public static class TitleDataManager {

    public static Dictionary<string, JobData> JobMasterData { get; private set; }

    public static Dictionary<string, JobTypeRewardRatesData> JobTypeRewardRatesMasterData { get; private set; }

    public static Dictionary<string, RewardData> RewardMasterData { get; set; }


    // TODO　マスターデータ用の変数を追加


    /// <summary>
    /// PlayFab のマスターデータ(TilteData) をローカルにキャッシュ
    /// </summary>
    /// <param name="titleData"></param>
    public static void SyncPlayFabToClient(Dictionary<string, string> titleData) {

        JobMasterData = JsonConvert.DeserializeObject<JobData[]>(titleData["JobMasterData"]).ToDictionary(x => x.jobTitle);

        Debug.Log("TitleData JobMasterData キャッシュ");

        JobTypeRewardRatesMasterData = JsonConvert.DeserializeObject<JobTypeRewardRatesData[]>(titleData["JobTypeRewardRatesMasterData"]).ToDictionary(x => x.jobType.ToString());

        Debug.Log("TitleData JobTypeRewardRatesMasterData キャッシュ");

        RewardMasterData = JsonConvert.DeserializeObject<RewardData[]>(titleData["RewardMasterData"]).ToDictionary(x => x.rewardName);

        Debug.Log("TitleData RewardMasterData キャッシュ");

        // Debug 用
        GameData.instance.SetMasterDatas();　　　//　<=　次の手順で作成するので、コメントアウトしておいてください。


        // TODO 他のマスターデータも追加

    }
}
