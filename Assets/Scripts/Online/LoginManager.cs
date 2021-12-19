using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System.Threading.Tasks;
using System;
using Cysharp.Threading.Tasks;

public static class LoginManager {�@�@//�@�Q�[�����s���ɃC���X�^���X�������I�ɂP�������������

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
}