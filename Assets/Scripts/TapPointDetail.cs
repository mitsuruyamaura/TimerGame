using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TapPointDetail : MonoBehaviour
{
    [SerializeField]
    private Button btnTapPoint;

    [SerializeField, Header("���̃^�b�v�|�C���g�̃W���u�ԍ�")]
    private int myJobNo;

    [SerializeField]
    private JobData jobData;

    private GameManager gameManager;

    void Start() {
        btnTapPoint.onClick.AddListener(OnClickTapPoint);
    }

    /// <summary>
    /// TapPointDetail �̐ݒ�@�@=>�@TODO UniRX �ɂ�����
    /// </summary>
    /// <param name="gameManager"></param>
    public void SetUpTapPointDetail(GameManager gameManager) {
        this.gameManager = gameManager;
    }

    /// <summary>
    /// �^�b�v�|�C���g���^�b�v�����ۂ̏���
    /// </summary>
    private void OnClickTapPoint() {
        Debug.Log("TapPoint �^�b�v");

        // �^�b�v�A�j��
        transform.DOPunchScale(Vector3.one * 1.25f, 0.15f).SetEase(Ease.OutBounce);

        // TODO �|�b�v�A�b�v�\�� ���̃N���X�̏���n��
        gameManager.OpenJobsConfirmPopUp(this);
    }

    /// <summary>
    /// �^�b�v�|�C���g�̕\��/��\���؂�ւ�
    /// </summary>
    /// <param name="isSwitch"></param>
    public void SwtichActivateTapPoint(bool isSwitch) {
        btnTapPoint.gameObject.SetActive(isSwitch);
    }
}
