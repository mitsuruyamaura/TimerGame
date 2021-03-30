using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

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
        // TODO ���g���̃f�[�^�̃��[�h
        OfflineTimeManager.instance.LoadOfflineJobTimeData(0);
    }

    void Start() {   // TODO �R���[�`���ɂ���

        // TODO �L�����̐����m�F

        TapPointSetUp();   
    }

    private void TapPointSetUp() {
        for (int i = 0; i < tapPointDetailsList.Count; i++) {
            tapPointDetailsList[i].SetUpTapPointDetail(this);

            // TODO ���[�h�����f�[�^���ƍ����āA���g�����̏ꍇ�ɂ͔�\���ɂ���
            if (JudgeJobsEnd(tapPointDetailsList[i])) {
                GenerateChara(tapPointDetailsList[i]);
            }
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

            // �d���J�n���Ԃ̃Z�[�u
            OfflineTimeManager.instance.SaveWorkingJobTimeData(selectedTapPointDetail.jobData.jobNo);

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

    private bool JudgeJobsEnd(TapPointDetail tapPointDetail) {

        // �ڕW�ƂȂ鎞�Ԃ� TimeSpan �ɂ���
        //TimeSpan addTime = new TimeSpan(0, 0, tapPointDetail.jobData.jobTime, 0);

        // TODO Offline �� List �ƈ����̏����m�F

        // ���g���J�n�̎��ԂɖڕW�l�����Z����
        //DateTime resultTime = OfflineTimeManager.instance.workingJobTimeDatasList[0].GetDateTime() + addTime;

        // ���݂̎��Ԃ��擾����
        DateTime currentDateTime = DateTime.Now;

        // �o�߂������Ԃ̍���
        TimeSpan timeElasped = currentDateTime - OfflineTimeManager.instance.workingJobTimeDatasList[0].GetDateTime();

        // �����l�� float �^�ɕϊ�
        float elapsedTime = (int)Math.Round(timeElasped.TotalSeconds, 0, MidpointRounding.ToEven);
        Debug.Log("���g�����Ԃ̍��� : " + elapsedTime + " : �b");

        // �o�ߎ��Ԃ����g���ɂ����鎞�Ԃ��������������Ȃ�
        if ((float)tapPointDetail.jobData.jobTime <= elapsedTime) {
            return true;
        }

        return false;
    }
}
