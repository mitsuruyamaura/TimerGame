using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private List<TapPointDetail> tapPointDetailsList = new List<TapPointDetail>();

    [SerializeField]
    private Transform canvasTran;

    [SerializeField]
    private JobsConfirmPopUp jobsConfirmPopUpPrefab;

    [SerializeField]
    private CharaController charaPrefab;

    [SerializeField]
    private List<CharaController> charasList = new List<CharaController>();

    private JobsConfirmPopUp jobsConfirmPopUp;

    private TapPointDetail selectedTapPointDetail;




    void Awake() {
        // TODO ���[�h

    }

    void Start() {   // TODO �R���[�`���ɂ���

        // TODO �L�����̐����m�F

        TapPointSetUp();   
    }

    private void TapPointSetUp() {
        for (int i = 0; i < tapPointDetailsList.Count; i++) {
            tapPointDetailsList[i].SetUpTapPointDetail(this);

            // TODO ���[�h�����f�[�^���ƍ����āA���g�����̏ꍇ�ɂ͔�\���ɂ���
        }
    }

    /// <summary>
    /// TapPoint ���N���b�N�����ۂɂ��g���m�F�p�̃|�b�v�A�b�v���J��
    /// </summary>
    /// <param name="tapPointDetail"></param>
    public void GenerateJobsConfirmPopUp(TapPointDetail tapPointDetail) {
        selectedTapPointDetail = tapPointDetail;

        // TODO �|�b�v�A�b�v���C���X�^���X���� 
        Debug.Log("���g���m�F�p�̃|�b�v�A�b�v���J��");

        jobsConfirmPopUp = Instantiate(jobsConfirmPopUpPrefab, canvasTran, false);

        // TODO �|�b�v�A�b�v�� JobData �𑗂�
        jobsConfirmPopUp.OpenPopUp(this, selectedTapPointDetail.jobData);
    }

    /// <summary>
    /// ���g���������󂯂����m�F
    /// </summary>
    /// <param name="isSubmit"></param>
    public void JudgeSubmitJob(bool isSubmit) {
        if (isSubmit) {
            // �{�^���̉摜��ύX
            selectedTapPointDetail.ChangeJobSprite();

            // �d�����̏�Ԃɂ���
            selectedTapPointDetail.isJobs = true;

            // �d���J�n
            StartCoroutine(selectedTapPointDetail.StartJobs());
        } else {
            Debug.Log("���g���ɂ͍s���Ȃ�");
        }
    }

    /// <summary>
    /// �L��������
    /// </summary>
    public void GenerateChara(TapPointDetail tapPointDetail) {
        selectedTapPointDetail = tapPointDetail;
        selectedTapPointDetail.SwtichActivateTapPoint(false);

        // TODO ���g���p�̃L�����̐���
        Debug.Log("���g���p�̃L�����̐���");
        CharaController chara = Instantiate(charaPrefab, selectedTapPointDetail.transform, false);

        chara.SetUpChara(this, selectedTapPointDetail);
    }

    /// <summary>
    /// �d�����̏�Ԃ��Z�[�u
    /// </summary>
    private void SaveJobs() {
        // �d���̔ԍ��@�d�����n�߂����Ԃ��L�^
    }

    /// <summary>
    /// �d�����̏�Ԃ����[�h
    /// </summary>
    private void LoadJobs() {

    }
}
