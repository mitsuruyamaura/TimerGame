using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class DisplayNameCanvas : MonoBehaviour
{
    [SerializeField]
    private Button btnSubmit;

    [SerializeField]
    private Button btnCancel;

    [SerializeField]
    private InputField displayNameInput;

    [SerializeField]
    private Text txtDisplayName;

    [SerializeField]
    private GameObject responsePopUp;

    [SerializeField]
    private Button btnClosePopUp;

    [SerializeField]
    private Text txtResponseInfo;

    private string displayName;�@�@// ���[�U�[���o�^�p


    void Start() {
        responsePopUp.SetActive(false);

        // �{�^���̓o�^
        btnSubmit?.OnClickAsObservable()
            .ThrottleFirst(TimeSpan.FromSeconds(0.5f))
            .Subscribe(_ => OnClickSubmit());

        btnCancel?.OnClickAsObservable()
            .ThrottleFirst(TimeSpan.FromSeconds(0.5f))
            .Subscribe(_ => OnCliclCancel());

        btnClosePopUp?.OnClickAsObservable()
            .ThrottleFirst(TimeSpan.FromSeconds(0.5f))
            .Subscribe(_ => OnClickCloseCompletePopUp());

        // InputField(�������͂��Ď����A��ʂ̕\���X�V���s��)
        displayNameInput?.OnEndEditAsObservable()
            .Subscribe(x => UpdateDispayName(x));
    }

    /// <summary>
    /// ���[�U�[���̒l�ƕ\���̍X�V
    /// </summary>
    /// <param name="newName"></param>
    private void UpdateDispayName(string newName) {
        txtDisplayName.text = newName;
        displayName = newName;
        Debug.Log(displayName);
    }

    /// <summary>
    /// OK �{�^�������������ۂ̏���
    /// </summary>
    private async void OnClickSubmit() {

        Debug.Log("OK �A�J�E���g�A�g�̏��F�J�n");

        // Email �ƃp�X���[�h�𗘗p���āA���[�U�[�A�J�E���g�̘A�g�����݂�
        (bool isSuccess, string message) response = await PlayerPlofileManager.UpdateUserDisplayNameAsync(displayName);

        // Debug�p
        if (response.isSuccess) {
            Debug.Log("���[�U�[���@�X�V����");
        } else {
            Debug.Log("���[�U�[���@�X�V���s");
        }

        txtResponseInfo.text = response.message;
        responsePopUp.SetActive(true);

    }

    /// <summary>
    /// NG �{�^�������������ۂ̏���
    /// </summary>
    private void OnCliclCancel() {
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
}
