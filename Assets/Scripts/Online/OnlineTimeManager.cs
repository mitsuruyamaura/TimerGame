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

    //    // UniTask �݂̂̏ꍇ(���݂� UniTask + UniRx �ɂĎ�����)
    //    // �L�����Z���[�V�����g�[�N���̍쐬
    //    var ct = this.GetCancellationTokenOnDestroy();

    //    // OnApplicationQuit ���\�b�h��񓯊��ɐݒ肷��
    //    await this.GetAsyncApplicationQuitTrigger().OnApplicationQuitAsync(ct);
    //}

    // Zenn �p�̃T���v���@UniTask + UniRx �ł� OnApplocationQuit ���\�b�h�̔񓯊������̑ҋ@����
    //private static async UniTask QuitGameAsync() {

    //    await UpdateLogOffTimeAsync();

    //    Debug.Log("�Z�[�u�����@�B");

    //    await UniTask.WhenAll(
    //        UpdateLogOffTimeAsync(),
    //        UpdateLogOffTimeAsync(),
    //        UpdateLogOffTimeAsync(),
    //        UpdateLogOffTimeAsync()
    //        );
    //    Debug.Log("�Z�[�u�����A�B");
    //    Debug.Log("�Q�[�����I�����܂��B");
    //}

    /// <summary>
    /// OnApplicationQuit ���Ɏ��s���郁�\�b�h�B����̓��O�I�t���Ԃ��T�[�o�[�ɃZ�[�u
    /// </summary>
    /// <returns></returns>
    public static async UniTask<bool> UpdateLogOffTimeAsync() {

        string dateTimeString = DateTime.Now.ToString(FORMAT);

        var request = new UpdateUserDataRequest {

            Data = new Dictionary<string, string> { { "LogOffTime", dateTimeString } }
        };

        var response = await PlayFabClientAPI.UpdateUserDataAsync(request);

        if (response.Error!= null) {
            Debug.Log("�G���[");
            return false;
        }

        Debug.Log("�T�[�o�[�Ƀ��O�I�t���̎����Z�[�u����");
        return true;
    }

    /// <summary>
    /// ���g���̎��Ԃ̃Z�[�u
    /// ���g���J�n���ƃQ�[���I�����ɃZ�[�u
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
            Debug.Log("�G���[");
            return;
        }

        Debug.Log("�T�[�o�[�Ɏd���̃f�[�^ �Z�[�u����");
    }
}
