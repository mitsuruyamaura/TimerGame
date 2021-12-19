using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using PlayFab;
using PlayFab.ClientModels;
using Newtonsoft.Json;


public static class UserDataManager {

    // TODO Level �Ȃǂ̏�����������

    public static User User { get; set; }

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

        // TODO ���ɂ�����������Βǉ�

    }
}