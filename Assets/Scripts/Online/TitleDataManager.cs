using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.Linq;


public static class TitleDataManager {

    public static Dictionary<string, JobData> JobMasterData { get; private set; }

    public static Dictionary<string, JobTypeRewardRatesData> JobTypeRewardRatesMasterData { get; private set; }

    public static Dictionary<string, RewardData> RewardMasterData { get; set; }


    // TODO�@�}�X�^�[�f�[�^�p�̕ϐ���ǉ�


    /// <summary>
    /// PlayFab �̃}�X�^�[�f�[�^(TilteData) �����[�J���ɃL���b�V��
    /// </summary>
    /// <param name="titleData"></param>
    public static void SyncPlayFabToClient(Dictionary<string, string> titleData) {

        JobMasterData = JsonConvert.DeserializeObject<JobData[]>(titleData["JobMasterData"]).ToDictionary(x => x.jobTitle);

        Debug.Log("TitleData JobMasterData �L���b�V��");

        JobTypeRewardRatesMasterData = JsonConvert.DeserializeObject<JobTypeRewardRatesData[]>(titleData["JobTypeRewardRatesMasterData"]).ToDictionary(x => x.jobType.ToString());

        Debug.Log("TitleData JobTypeRewardRatesMasterData �L���b�V��");

        RewardMasterData = JsonConvert.DeserializeObject<RewardData[]>(titleData["RewardMasterData"]).ToDictionary(x => x.rewardName);

        Debug.Log("TitleData RewardMasterData �L���b�V��");

        // Debug �p
        GameData.instance.SetMasterDatas();�@�@�@//�@<=�@���̎菇�ō쐬����̂ŁA�R�����g�A�E�g���Ă����Ă��������B


        // TODO ���̃}�X�^�[�f�[�^���ǉ�

    }
}
