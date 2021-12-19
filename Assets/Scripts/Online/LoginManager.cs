using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System.Threading.Tasks;
using System;
using Cysharp.Threading.Tasks;

public static class LoginManager {�@�@//�@�Q�[�����s���ɃC���X�^���X�������I�ɂP�������������

    /// <summary>
    /// ���O�C���Ɠ����� PlayFab ����擾������̐ݒ�p�N���X�ł��� GetPlayerCombinedInfoRequestParams �̃v���p�e�B�B
    /// GetPlayerCombinedInfoRequestParams �N���X�Őݒ肵���l�� InfoRequestParameters �̐ݒ�l�ɂȂ�Atrue �ɂ��Ă��鍀�ڂŊe��񂪎����I�Ɏ擾�ł���悤�ɂȂ�
    /// �e�p�����[�^�̏����l�͂��ׂ� false
    /// �擾�������Ȃ�قǃ��O�C�����Ԃ�������A�������������̂ŋC��t����
    /// �擾���ʂ� InfoResultPayLoad �ɓ����Ă���Bfalse �̂��̂͂��ׂ� null �ɂȂ�
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
    /// �R���X�g���N�^
    /// </summary>
    static LoginManager() {

        // TitleId �ݒ�
        PlayFabSettings.staticSettings.TitleId = "457D7";

        Debug.Log("TitleID �ݒ�: " + PlayFabSettings.staticSettings.TitleId);
    }

    /// <summary>
    /// ����������
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static async UniTaskVoid InitializeAsync() {

        Debug.Log("�������J�n");

        // PlayFab �ւ̃��O�C�������ƃ��O�C��
        await PrepareLoginPlayPab();

        Debug.Log("����������");
    }

    /// <summary>
    /// PlayFab �ւ̃��O�C�������ƃ��O�C��
    /// </summary>
    public static async UniTask PrepareLoginPlayPab() {

        Debug.Log("���O�C�� ���� �J�n");

        // ���̃��O�C���̏��(���N�G�X�g)���쐬���Đݒ�
        var request = new LoginWithCustomIDRequest {
            CustomId = "GettingStartedGuide",�@�@�@�@�@// ���̕��������[�U�[��ID�ɂȂ�܂�
            CreateAccount = true                       // �A�J�E���g���쐬����Ă��Ȃ��ꍇ�Atrue �̏ꍇ�͓������O�C�����ăA�J�E���g���쐬����
        };

        // PlayFab �փ��O�C���B��񂪊m�F�ł���܂őҋ@
        var result = await PlayFabClientAPI.LoginWithCustomIDAsync(request);

        // �G���[�̓��e�����āA���O�C���ɐ������Ă��邩�𔻒�(�G���[�n���h�����O)
        var message = result.Error is null
            ? $"���O�C������! My PlayFabID is { result.Result.PlayFabId }"    // Error �� null �Ȃ��[�G���[�Ȃ�]�Ȃ̂ŁA���O�C������
            : result.Error.GenerateErrorReport();                             // Error �� null �ȊO�̏ꍇ�̓G���[���������Ă���̂ŁA���|�[�g�쐬

        Debug.Log(message);
    }

    /// <summary>
    /// Email �ƃp�X���[�h�Ń��O�C��(�A�J�E���g�񕜗p)
    /// </summary>
    /// <param name="email"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public static async UniTask<(bool, string)> LoginEmailAndPasswordAsync(string email, string password) {

        // Email �ɂ�郍�O�C�����N�G�X�g�̍쐬
        var request = new LoginWithEmailAddressRequest {
            Email = email,
            Password = password,
            InfoRequestParameters = CombinedInfoRequestParams
        };

        // PlayFab �Ƀ��O�C��
        var response = await PlayFabClientAPI.LoginWithEmailAddressAsync(request);

        // �G���[�n���h�����O
        if (response.Error != null) {
            switch (response.Error.Error) {
                case PlayFabErrorCode.InvalidParams:
                case PlayFabErrorCode.InvalidEmailOrPassword:
                case PlayFabErrorCode.AccountNotFound:
                    Debug.Log("���[���A�h���X���p�X���[�h������������܂���");
                    break;
                default:
                    Debug.Log(response.Error.GenerateErrorReport());
                    break;
            }

            return (false, "���[���A�h���X���p�X���[�h������������܂���");
        }

        // PlayerPrefas �����������āA���O�C�����ʂ� UserId ��o�^������
        PlayerPrefs.DeleteAll();

        // �V���� PlayFab ���� UserId ���擾
        // InfoResultPayload �̓N���C�A���g�v���t�B�[���I�v�V����(InfoRequestParameters)�ŋ�����ĂȂ��� null �ɂȂ�
        PlayerPrefsManager.UserId = response.Result.InfoResultPayload.AccountInfo.CustomIdInfo.CustomId;

        // Email �Ń��O�C���������Ƃ��L�^����
        PlayerPrefsManager.IsLoginEmailAdress = true;

        return (true, "Email �ɂ�郍�O�C�����������܂����B");
    }
}