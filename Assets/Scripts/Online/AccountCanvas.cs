using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System;

public class AccountCanvas : MonoBehaviour
{
    [SerializeField]
    private Button btnSubmit;

    [SerializeField]
    private Button btnCancel;

    [SerializeField]
    private InputField emailInput;

    [SerializeField]
    private InputField passwordInput;

    [SerializeField]
    private Text txtEmail;

    [SerializeField]
    private Text txtPassword;

    [SerializeField]
    private GameObject responsePopUp;

    [SerializeField]
    private Button btnClosePopUp;

    [SerializeField]
    private Button btnEmainLogin;

    [SerializeField]
    private Text txtResponseInfo;

    private (string email, string pasword) inputValue;�@�@// Email �ƃp�X���[�h�o�^�p


    void Start()
    {
        responsePopUp.SetActive(false);

        // �{�^���̓o�^
        btnSubmit?.OnClickAsObservable()
            .ThrottleFirst(TimeSpan.FromSeconds(0.5f))
            .Subscribe(_ => OnClickSubmit());

        btnCancel?.OnClickAsObservable()
            .ThrottleFirst(TimeSpan.FromSeconds(0.5f))
            .Subscribe(_ => OnClickCancel());

        btnClosePopUp?.OnClickAsObservable()
            .ThrottleFirst(TimeSpan.FromSeconds(0.5f))
            .Subscribe(_ => OnClickCloseCompletePopUp());

        btnEmainLogin?.OnClickAsObservable()
            .ThrottleFirst(TimeSpan.FromSeconds(0.5f))
            .Subscribe(_ => OnClickEmailLogin());

        // InputField
        emailInput?.OnEndEditAsObservable()
            .Subscribe(x => UpdateDispayEmail(x));

        passwordInput?.OnEndEditAsObservable()
            .Subscribe(x => UpdateDisplayPassword(x));
    }

    /// <summary>
    /// Email �̒l�ƕ\���̍X�V
    /// </summary>
    /// <param name="newEmail"></param>
    private void UpdateDispayEmail(string newEmail) {
        txtEmail.text = newEmail;
        inputValue.email = newEmail;
        Debug.Log(inputValue);
    }

    /// <summary>
    /// �p�X���[�h�̒l�ƕ\���̍X�V
    /// </summary>
    /// <param name="newPassword"></param>
    private void UpdateDisplayPassword(string newPassword) {
        txtPassword.text = newPassword;
        inputValue.pasword = newPassword;
        Debug.Log(inputValue);
    }

    /// <summary>
    /// OK �{�^�������������ۂ̏���
    /// </summary>
    private async void OnClickSubmit() {

        Debug.Log("OK �A�J�E���g�A�g�̏��F�J�n");

        // Email �ƃp�X���[�h�𗘗p���āA���[�U�[�A�J�E���g�̘A�g�����݂�
        bool isLink = await PlayFabAccountLink.SetEmailAndPasswordAsync(inputValue.email, inputValue.pasword);

        if (isLink) {
            Debug.Log("�A�g����");

            txtResponseInfo.text = "�A�J�E���g�A�g���������܂����B";
            responsePopUp.SetActive(true);
        } else {
            Debug.Log("�A�J�E���g�A�g�����s���܂����B");

            txtResponseInfo.text = "�A�J�E���g�A�g�����s���܂����B";
            responsePopUp.SetActive(true);
        }
    }

    /// <summary>
    /// NG �{�^�������������ۂ̏���
    /// </summary>
    private void OnClickCancel() {
        this.gameObject.SetActive(false);

        Debug.Log("NG");
    }

    /// <summary>
    /// CompletePopUp ���^�b�v�����ۂ̏���
    /// </summary>
    private void OnClickCloseCompletePopUp() {

        responsePopUp.SetActive(false);

        this.gameObject.SetActive(false);
    }

    /// <summary>
    /// Email �Ń��O�C���{�^�������������ۂ̏���
    /// </summary>
    private async void OnClickEmailLogin() {

        // Email �Ń��O�C�������݂� isLogin = true �Ȃ烍�O�C������
        (bool isLogin, string log) response = await LoginManager.LoginEmailAndPasswordAsync(inputValue.email, inputValue.pasword);

        // TODO isLogin �ŕ��򂳂��Ă�����

        Debug.Log(response.log);

        txtResponseInfo.text = response.log;
        responsePopUp.SetActive(true);

        return;
    }
}
