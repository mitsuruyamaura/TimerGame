using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using PlayFab;
using PlayFab.ClientModels;
using Newtonsoft.Json;
using System;


public static class UserDataManager {

    // TODO Level �Ȃǂ̏�����������

    public static User User { get; set; }

    private const string FORMAT = "yyyy/MM/dd HH:mm:ss";

    public static Reward Reward { get; set; }



    /// <summary>
    /// �v���C���[�f�[�^���̍쐬�ƍX�V(�v���C���[�f�[�^(�^�C�g��)�� Key �ɂP�����l��o�^������@)
    /// </summary>
    /// <param name="updateUserData"></param>
    /// <param name="userDataPermission"></param>
    public static async UniTask UpdatePlayerDataAsync(Dictionary<string, string> updateUserData, UserDataPermission userDataPermission = UserDataPermission.Private) {

        var request = new UpdateUserDataRequest {
            Data = updateUserData,

            // �A�N�Z�X���̕ύX
            Permission = userDataPermission
        };

        var response = await PlayFabClientAPI.UpdateUserDataAsync(request);

        if (response.Error != null) {

            Debug.Log("�G���[");
            return;
        }

        Debug.Log("�v���C���[�f�[�^�@�X�V");
    }

    /// <summary>
    /// �v���C���[�f�[�^����w�肵�� Key �̏��̍폜
    /// </summary>
    /// <param name="deleteKey">�폜���� Key �̖��O</param>
    public static async void DeletePlayerDataAsync(string deleteKey) {

        var request = new UpdateUserDataRequest {
            KeysToRemove = new List<string> { deleteKey }
        };

        var response = await PlayFabClientAPI.UpdateUserDataAsync(request);

        if (response.Error != null) {

            Debug.Log("�G���[");
            return;
        }

        Debug.Log("�v���C���[�f�[�^�@�폜");
    }

    /// <summary>
    /// �v���C���[�f�[�^�̍쐬�ƍX�V(�v���C���[�f�[�^(�^�C�g��)�� Key �ɕ����̒l�����܂Ƃ߂� Json �𗘗p����ꍇ)
    /// </summary>
    /// <param name="userName">key</param>
    /// <param name="userDataPermission"></param>
    /// <returns></returns>
    public static async UniTask<(bool isSuccess, string errorMessage)> UpdateUserDataByJsonAsync(string userName, UserDataPermission userDataPermission = UserDataPermission.Private) {

        string userJson = JsonConvert.SerializeObject(User);�@�@�@//�@<=�@���̋@�\�� Json.NET ���C�u�����̏����ł��B

        var request = new UpdateUserDataRequest {
            Data = new Dictionary<string, string> {
                { userName, userJson }
            },

            // �A�N�Z�X���̕ύX
            Permission = userDataPermission
        };

        var response = await PlayFabClientAPI.UpdateUserDataAsync(request);

        if (response.Error != null) {

            Debug.Log("�G���[");
            return (false, response.Error.ToString());
        }

        return (true, string.Empty);
    }

    /// <summary>
    /// PlayFab �̍ŐV�f�[�^���擾���ă��[�J���ɃL���b�V��
    /// </summary>
    /// <param name="userData"></param>
    public static void SyncPlayFabToClient(Dictionary<string, UserDataRecord> userData) {

        // ���[�U�[�̏����擾�B�擾�ł����ꍇ�ɂ͕������A�擾�ł��Ȃ��ꍇ�ɂ͐V�K���[�U�[�̍쐬
        User = userData.TryGetValue("User", out var user)
            ? JsonConvert.DeserializeObject<User>(user.Value) : User.Create();

        Debug.Log("PlayFab �̃��[�U�[�f�[�^���擾");

        // ���O�I�t�������Ԃ̎擾
        if (userData.TryGetValue("LogOffTime", out var logOffTime)) {

            OfflineTimeManager.instance.LoadDateTime = DateTime.ParseExact(logOffTime.Value, FORMAT, null);
            Debug.Log($"�O�񃍃O�I�t�������� : { logOffTime.Value }");
        }

        // �d���̏����擾
        if (userData.TryGetValue("JobTimes", out var jobTimes)) {
            OfflineTimeManager.instance.workingJobTimeDatasList = JsonConvert.DeserializeObject<List<OfflineTimeManager.JobTimeData>>(jobTimes.Value);
        } else {
            Debug.Log("�d���̃f�[�^�Ȃ�");
        }

        Debug.Log("PlayFab �̎d���̃f�[�^���擾");

        // �I�����C���Ōo�߂������Ԃ��v�Z
        //OfflineTimeManager.instance.CalculateOfflineDateTimeElasped(OfflineTimeManager.instance.LoadDateTime);


        // TODO ���ɂ�����������Βǉ�

    }

    /// <summary>
    /// �l�������J�܂̒ǉ��E���Z
    /// </summary>
    /// <param name="rewardData"></param>
    /// <param name="addCount"></param>
    public static void AddReward(RewardData rewardData, int addCount = 1) {

        // Reward ���C���X�^���X����Ă��Ȃ��ꍇ�ɂ͏�����
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
    /// �l�������J�܂̃f�[�^�̍X�V
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
            Debug.Log("�G���[");
            return (false, response.Error.ToString());
        }

        return (true, string.Empty);
    }



}