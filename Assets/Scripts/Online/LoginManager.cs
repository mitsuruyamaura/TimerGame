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

    public static bool isSetup;

    /// <summary>
    /// ����������
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static async UniTaskVoid InitializeAsync() {

        //Debug.Log("�������J�n");
        isSetup = false;

        // PlayFab �ւ̃��O�C�������ƃ��O�C��
        await PrepareLoginPlayPab();

        Debug.Log("����������");

        isSetup = true;
    }

    /// <summary>
    /// PlayFab �ւ̃��O�C�������ƃ��O�C��
    /// </summary>
    public static async UniTask PrepareLoginPlayPab() {

        Debug.Log("���O�C�� ���� �J�n");

        await LoginAndUpdateLocalCacheAsync();
    }

    /// <summary>
    /// ���[�U�[�f�[�^�ƃ^�C�g���f�[�^��������
    /// </summary>
    /// <returns></returns>
    public static async UniTask LoginAndUpdateLocalCacheAsync() {

        Debug.Log("�������J�n");

        // ���[�U�[ID �̎擾�����݂�
        var userId = PlayerPrefsManager.UserId;

        // ���[�U�[ID ���擾�ł��Ȃ��ꍇ�ɂ͐V�K�쐬���ē������O�C������
        // �擾�ł����ꍇ�ɂ́A���[�U�[ID ���g���ă��O�C������
        var loginResult = string.IsNullOrEmpty(userId)
            ? await CreateNewUserAsync() : await LoadUserAsync(userId);

        // �v���C���[�f�[�^�̍쐬�ƍX�V
        //await CreateUserDataAsync();

        // PlayFab �̃f�[�^�������Ŏ擾����ݒ�ɂ��Ă���̂ŁA�擾�����f�[�^�����[�J���ɃL���b�V������
        UpdateLocalCacheAsync(loginResult);
    }

    /// <summary>
    /// �V�K���[�U�[���쐬���� UserId �� PlayerPrefs �ɕۑ�
    /// </summary>
    /// <returns></returns>
    private static async UniTask<LoginResult> CreateNewUserAsync() {

        Debug.Log("���[�U�[�f�[�^�Ȃ��B�V�K���[�U�[�쐬");

        while (true) {

            // UserId �̍̔�
            var newUserId = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 20);

            // ���O�C�����N�G�X�g�̍쐬
            var request = new LoginWithCustomIDRequest {
                CustomId = newUserId,
                CreateAccount = true,
                InfoRequestParameters = CombinedInfoRequestParams
            };

            // PlayFab �Ƀ��O�C��
            var response = await PlayFabClientAPI.LoginWithCustomIDAsync(request);

            // �G���[���Ȃ����m�F
            if (response.Error != null) {
                Debug.Log("Error");
            }

            // ������ LastLoginTime �ɒl�������Ă���ꍇ�ɂ́A�̔Ԃ��� ID ���������[�U�[�Əd�����Ă���̂Ń��g���C����
            if (response.Result.LastLoginTime.HasValue) {
                continue;
            }

            // PlayerPrefs �� UserId ���L�^����
            PlayerPrefsManager.UserId = newUserId;

            return response.Result;
        }
    }

    /// <summary>
    /// ���O�C�����ă��[�U�[�f�[�^�����[�h
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    private static async UniTask<LoginResult> LoadUserAsync(string userId) {

        Debug.Log("���[�U�[�f�[�^����B���O�C���J�n");

        // ���O�C�����N�G�X�g�̍쐬
        var request = new LoginWithCustomIDRequest {
            CustomId = userId,
            CreateAccount = false,
            InfoRequestParameters = CombinedInfoRequestParams
        };

        // PlayFab �Ƀ��O�C��
        var response = await PlayFabClientAPI.LoginWithCustomIDAsync(request);

        // �G���[�n���h�����O
        if (response.Error != null) {
            Debug.Log("Error");

            // TODO response.Error �ɂ̓G���[�̎�ނ��l�Ƃ��ē����Ă���
            // ���̃G���[�ɑΉ����������� switch ���ȂǂŋL�q���ĕ����̃G���[�ɑΉ��ł���悤�ɂ���


        }

        // �G���[�̓��e�����ăn���h�����O���s���A���O�C���ɐ������Ă��邩�𔻒�
        var message = response.Error is null ? $"Login success! My PlayFabID is {response.Result.PlayFabId}" : response.Error.GenerateErrorReport();

        Debug.Log(message);

        return response.Result;
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

    /// <summary>
    /// �v���C���[�f�[�^�̍쐬�ƍX�V�B�f�o�b�O�p
    /// </summary>
    private static async UniTask CreateUserDataAsync() {

        UserDataManager.User = User.Create();      //  �V���� User �쐬���܂�
        string key = "User";�@�@�@�@�@�@�@�@�@�@�@ //�@�ۑ��p�� Key ���쐬���܂�

        await UserDataManager.UpdateUserDataByJsonAsync(key);   // PlayFab �� Json �`���ɂ��� User �N���X�̏���o�^���܂�

        Debug.Log("���[�U�[�f�[�^ �o�^����");
    }

    /// <summary>
    /// PlayFab ����擾�����f�[�^�Q�����[�J��(�[��)�ɃL���b�V��
    /// </summary>
    /// <param name="loginResult"></param>
    /// <returns></returns>

    public static void UpdateLocalCacheAsync(LoginResult loginResult) {

        // TODO �J�^���O�ނ̏������B���̃C���X�^���X�̏������ɂ��K�v�Ȃ̂ōŏ��ɍs��


        // �^�C�g���f�[�^�̎擾
        TitleDataManager.SyncPlayFabToClient(loginResult.InfoResultPayload.TitleData);


        // ���[�U�[�f�[�^�̎擾
        UserDataManager.SyncPlayFabToClient(loginResult.InfoResultPayload.UserData);

        // ���[�U�[���Ȃǂ̎擾
        PlayerPlofileManager.SyncPlayFabToClient(loginResult.InfoResultPayload.PlayerProfile, loginResult.InfoResultPayload.PlayerStatistics);

        // TODO ���̏�����������ǉ�


        Debug.Log("�e��f�[�^�̃L���b�V������");
    }
}