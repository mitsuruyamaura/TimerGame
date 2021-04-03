using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using System.Linq;

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

    [SerializeField]
    private RewardDataSO rewardDataSO;

    [SerializeField]
    private JobTypeRewardRatesDataSO JobTypeRewardRatesDataSO;

    [SerializeField]
    private RewardPopUp rewardPopUpPrefab;

    private JobsConfirmPopUp jobsConfirmPopUp;


    void Start() {   // TODO �R���[�`���ɂ���
        OfflineTimeManager.instance.SetGameManager(this);

        // TODO ���g���̃f�[�^�̃��[�h
        OfflineTimeManager.instance.LoadOfflineJobTimeData(0);

        // TODO �L�����̐����m�F

        TapPointSetUp();   
    }

    /// <summary>
    /// �e TapPointDetail �̐ݒ�B���g���̏󋵂ɍ��킹�āA�d�������d���I�����𔻒f���ăL�����𐶐����邩�A���g�����ĊJ���邩����
    /// </summary>
    private void TapPointSetUp() {
        for (int i = 0; i < tapPointDetailsList.Count; i++) {
            tapPointDetailsList[i].SetUpTapPointDetail(this);

            // TapPointDetail �ɓo�^����Ă��� JobData �ɊY������ JobTimeData ���擾
            OfflineTimeManager.JobTimeData jobTime = OfflineTimeManager.instance.workingJobTimeDatasList.Find((x) => x.jobNo == tapPointDetailsList[i].jobData.jobNo);

            // ���g�����łȂ���Ύ��̏����ֈڂ�
            if (jobTime == null) {
                continue;
            }

            // TODO ���[�h�����f�[�^���ƍ����āA���g�����̏ꍇ�ɂ͔�\���ɂ���
            if (JudgeJobsEnd(jobTime)) {
                // TODO ���g���̃��X�g�ƃZ�[�u�f�[�^���폜�@�L�������^�b�v���Ă������
                //OfflineTimeManager.instance.RemoveWorkingJobTimeDatasList(jobTime.jobNo);

                // ���g���I���B�L�����������Č��ʂ��m�F
                GenerateChara(tapPointDetailsList[i]);
            } else {
                // ���g���ĊJ
                JudgeSubmitJob(true, tapPointDetailsList[i]);
            }
        }
    }

    /// <summary>
    /// TapPoint ���N���b�N�����ۂɂ��g���m�F�p�̃|�b�v�A�b�v���J��
    /// </summary>
    /// <param name="tapPointDetail"></param>
    public void GenerateJobsConfirmPopUp(TapPointDetail tapPointDetail) {

        // TODO �|�b�v�A�b�v���C���X�^���X���� 
        Debug.Log("���g���m�F�p�̃|�b�v�A�b�v���J��");

        jobsConfirmPopUp = Instantiate(jobsConfirmPopUpPrefab, canvasTran, false);

        // TODO �|�b�v�A�b�v�� JobData �𑗂�
        jobsConfirmPopUp.OpenPopUp(this, tapPointDetail);
    }

    /// <summary>
    /// ���g���������󂯂����m�F
    /// </summary>
    /// <param name="isSubmit"></param>
    public void JudgeSubmitJob(bool isSubmit, TapPointDetail tapPointDetail) {
        if (isSubmit) {
            // �{�^���̉摜��ύX
            tapPointDetail.ChangeJobSprite();

            // �d�����̏�Ԃɂ���
            tapPointDetail.isJobs = true;

            // �d���̓o�^
            OfflineTimeManager.instance.CreateWorkingJobTimeDatasList(tapPointDetail);

            // �d���J�n���Ԃ̃Z�[�u
            OfflineTimeManager.instance.SaveWorkingJobTimeData(tapPointDetail.jobData.jobNo);

            // �d���J�n
            StartCoroutine(tapPointDetail.WorkingJobs(tapPointDetail.jobData.jobTime));
        } else {
            Debug.Log("���g���ɂ͍s���Ȃ�");
        }
    }

    /// <summary>
    /// �L��������
    /// </summary>
    public void GenerateChara(TapPointDetail tapPointDetail) {
        tapPointDetail.SwtichActivateTapPoint(false);

        // TODO ���g���p�̃L�����̐���
        Debug.Log("���g���p�̃L�����̐���");
        CharaController chara = Instantiate(charaPrefab, tapPointDetail.transform, false);

        chara.SetUpChara(this, tapPointDetail);
    }

    /// <summary>
    /// ���Ԃ̍������A���g�����I�����Ă��邩����
    /// </summary>
    /// <param name="jobTimeData"></param>
    /// <returns></returns>
    private bool JudgeJobsEnd(OfflineTimeManager.JobTimeData jobTimeData) {

        // �ڕW�ƂȂ鎞�Ԃ� TimeSpan �ɂ���
        //TimeSpan addTime = new TimeSpan(0, 0, tapPointDetail.jobData.jobTime, 0);

        // TODO Offline �� List �ƈ����̏����m�F

        // ���g���J�n�̎��ԂɖڕW�l�����Z����
        //DateTime resultTime = OfflineTimeManager.instance.workingJobTimeDatasList[0].GetDateTime() + addTime;

        // ���݂̎��Ԃ��擾����
        DateTime currentDateTime = DateTime.Now;

        // ���g�����J�n�������ԂƁA���݂̎��Ԃ��v�Z���āA�o�߂������Ԃ̍������擾
        TimeSpan timeElasped = currentDateTime - jobTimeData.GetDateTime();

        // �����l�� float �^�ɕϊ�
        float elapsedTime = (int)Math.Round(timeElasped.TotalSeconds, 0, MidpointRounding.ToEven);
        Debug.Log("���g�����Ԃ̍��� : " + elapsedTime + " : �b");

        // �o�ߎ��Ԃ����g���ɂ����鎞�Ԃ��������������Ȃ�
        if ((float)jobTimeData.elespedJobTime <= elapsedTime) {
            return true;
        }

        return false;
    }

    /// <summary>
    /// �w�肵�����g���̔ԍ��́A���݂̎c�莞�Ԃ��擾
    /// </summary>
    /// <param name="jobNo"></param>
    /// <returns></returns>
    public int GetTapPointDetailCurrentJobTime(int jobNo) {
        return tapPointDetailsList.Find(x => x.jobData.jobNo == jobNo).GetCurrentJobTime();
    }

    /// <summary>
    /// ���g���̐��ʔ��\
    /// �L�������^�b�v����ƌĂяo��
    /// </summary>
    public void ResultJobs(TapPointDetail tapPointDetail) {

        // TODO ����
        Debug.Log("���� ���\");

        // ���g���̓������J�܌���
        RewardData rewardData = GetLotteryForRrewards(tapPointDetail.jobData.jobType);
        Debug.Log(rewardData.rewardNo);

        // TODO �|�b�v�A�b�v�\��
        Debug.Log("�|�b�v�A�b�v�\��");
        // ���ʃE�C���h�E����
        //RewardPopUp rewardPopUp = Instantiate(rewardPopUpPrefab, canvasTran, false);
        //rewardPopUp.SetUpRewardPopUp(rewardData);

        // TapPoint �̏�Ԃ��ēx�������Ԃɖ߂�
        tapPointDetail.SwtichActivateTapPoint(true);

        // �摜�����̉摜�ɖ߂�
        tapPointDetail.ChangeDefaultSprite();

        // TODO ���g���̃��X�g�ƃZ�[�u�f�[�^���폜�@�L�������^�b�v���Ă������
        OfflineTimeManager.instance.RemoveWorkingJobTimeDatasList(tapPointDetail.jobData.jobNo);
    }

    /// <summary>
    /// ���g���̂��J���̒��I
    /// </summary>
    private RewardData GetLotteryForRrewards(JobType jobType) {
        // ��Փx�ɂ��󏭓x�̍��v�l���Z�o���āA�����_���Ȓl�𒊏o
        int randomRarityValue = UnityEngine.Random.Range(0, JobTypeRewardRatesDataSO.jobTypeRewardRatesDataList[(int)jobType].rewardRates.Sum());

        Debug.Log(JobTypeRewardRatesDataSO.jobTypeRewardRatesDataList[(int)jobType].rewardRates.Sum());

        RarityType rarityType = RarityType.Common;
        int total = 0;
        Debug.Log(randomRarityValue);

        // ���o�����l���ǂ̊󏭓x�ɂȂ邩�m�F
        for (int i = 0; i < JobTypeRewardRatesDataSO.jobTypeRewardRatesDataList.Count; i++) {
            total += JobTypeRewardRatesDataSO.jobTypeRewardRatesDataList[(int)jobType].rewardRates[i];
            Debug.Log(total);
            // 
            if (randomRarityValue <= total) {
                // 
                rarityType = (RarityType)i;
                break; 
            }
        }

        Debug.Log(rarityType);

        // ����ΏۂƂȂ�󏭓x�̃f�[�^�����̃��X�g���쐬
        List<RewardData> rewardDatas = new List<RewardData>(rewardDataSO.rewardDatasList.Where(x => x.rarityType == rarityType).ToList());

        // �����󏭓x�̍��v�l���Z�o���āA�����_���Ȓl�𒊏o
        int randomRewardValue = UnityEngine.Random.Range(0, rewardDatas.Select(x => x.rarityRate).ToArray().Sum());

        Debug.Log(randomRewardValue);

        // �ǂ̖J�܂ɂȂ邩�m�F

        total = 0;
        // ���o�����l���ǂ̖J�܂ɂȂ邩�m�F
        for (int i = 0; i < rewardDatas.Count; i++) {
            total += rewardDatas[i].rarityRate;

            if (randomRewardValue <= total) {
                return rewardDatas[i];
            }
        }
        return null;
    }
}
