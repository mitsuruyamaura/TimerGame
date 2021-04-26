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
    private CharaDetail charaDetailsPrefab;

    [SerializeField]
    private List<CharaDetail> charaDetailsList = new List<CharaDetail>();

    [SerializeField]
    private RewardPopUp rewardPopUpPrefab;

    [SerializeField]
    private RewardDataSO rewardDataSO;

    [SerializeField]
    private JobTypeRewardRatesDataSO JobTypeRewardRatesDataSO;

    [SerializeField]
    private UnityEngine.UI.Button btnAlbum;

    [SerializeField]
    private AlbumPopUp AlbumPopUpPrefab;

    private AlbumPopUp albumPopUp;

    [SerializeField]
    private JobDataSO jobDataSO;


    void Start() {   // TODO �R���[�`���ɂ���
        //OfflineTimeManager.instance.SetGameManager(this);

        // �J�܃f�[�^�̍ő吔��o�^
        GameData.instance.GetMaxRewardDataCount(rewardDataSO.rewardDatasList.Count);

        // �l�����Ă���J�܃f�[�^�̊m�F�ƃ��[�h
        GameData.instance.LoadEarnedRewardData();

        // �l�����Ă���J�܃f�[�^������ꍇ
        if (GameData.instance.GetEarnedRewardsListCount() > 0) {
            // �J�܃|�C���g�����[�h
            GameData.instance.LoadTotalRewardPoint();
        }
        
        btnAlbum.onClick.AddListener(OnClickAlbum);

        // �e TapPointDetail �� JobData ��ݒ�
        SetUpJobDatasToTapPointDetails();

        // ���g���̃f�[�^�̃��[�h
        OfflineTimeManager.instance.GetWorkingJobTimeDatasList(tapPointDetailsList);

        // �e TapPointDetail �̐ݒ�
        JudgeCompleteJobs();        
    }

    /// <summary>
    /// �e TapPointDetail �� JobData ��ݒ�
    /// </summary>
    private void SetUpJobDatasToTapPointDetails() {
        for (int i = 0; i < tapPointDetailsList.Count; i++) {
            JobData jobData = jobDataSO.jobDatasList.Find(x => x.jobNo == tapPointDetailsList[i].GetMyJobNo());
            tapPointDetailsList[i].SetUpTapPointDetail(this, jobData);
        }
    }

    /// <summary>
    /// �Z�[�u����Ă��邨�g���̎��ԃf�[�^��
    /// </summary>
    private void LoadJobTimeDatas() {

    }

    /// <summary>
    /// �e TapPointDetail �̂��g���̏󋵂ɍ��킹�āA�d�������d���I�����𔻒f���ăL�����𐶐����邩�A���g�����ĊJ���邩����
    /// </summary>
    private void JudgeCompleteJobs() {
        for (int i = 0; i < tapPointDetailsList.Count; i++) {
            //JobData jobData = jobDataSO.jobDatasList.Find(x => x.jobNo == tapPointDetailsList[i].GetMyJobNo());

            //tapPointDetailsList[i].SetUpTapPointDetail(this, jobData);

            // TapPointDetail �ɓo�^����Ă��� JobData �ɊY������ JobTimeData ���擾
            OfflineTimeManager.JobTimeData jobTime = OfflineTimeManager.instance.workingJobTimeDatasList.Find((x) => x.jobNo == tapPointDetailsList[i].jobData.jobNo);

            // ���g�����łȂ���Ύ��̏����ֈڂ�
            if (jobTime == null) {
                continue;
            }

            // ���g���̏�ԂƎc�莞�Ԃ��擾
            (bool isJobEnd, int remainingTime) = JudgeJobsEnd(jobTime);
            Debug.Log(remainingTime);

            // TODO ���[�h�����f�[�^���ƍ����āA���g�����̏ꍇ�ɂ͔�\���ɂ���
            if (isJobEnd) {
                // TODO ���g���̃��X�g�ƃZ�[�u�f�[�^���폜�@�L�������^�b�v���Ă������
                OfflineTimeManager.instance.RemoveWorkingJobTimeDatasList(jobTime.jobNo);

                // ���g���I���B�L�����������Č��ʂ��m�F
                GenerateCharaDetail(tapPointDetailsList[i]);
            } else {
                // ���g���ĊJ
                JudgeSubmitJob(true, tapPointDetailsList[i], remainingTime);
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

        JobsConfirmPopUp jobsConfirmPopUp = Instantiate(jobsConfirmPopUpPrefab, canvasTran, false);

        // TODO �|�b�v�A�b�v�� JobData �𑗂�
        jobsConfirmPopUp.OpenPopUp(tapPointDetail, this);
    }

    /// <summary>
    /// ���g���������󂯂����m�F
    /// </summary>
    /// <param name="isSubmit"></param>
    public void JudgeSubmitJob(bool isSubmit, TapPointDetail tapPointDetail, int remainingTime = -1) {
        if (isSubmit) {
            // �{�^���̉摜��ύX
            //tapPointDetail.ChangeJobSprite();

            // �d�����̏�Ԃɂ���
            //tapPointDetail.IsJobs = true;

            // �d���̓o�^
            OfflineTimeManager.instance.CreateWorkingJobTimeDatasList(tapPointDetail);

            // �d���J�n���Ԃ̃Z�[�u
            OfflineTimeManager.instance.SaveWorkingJobTimeData(tapPointDetail.jobData.jobNo);

            // ���g���̏���
            if (remainingTime == -1) {
                // ���܂܂ł��g�����Ă��Ȃ��Ȃ�A���g�����Ƃ̏����l�̂��g���̎��Ԃ�ݒ�
                tapPointDetail.PrapareteJobs(tapPointDetail.jobData.jobTime);
            } else {
                // ���g���̓r���̏ꍇ�ɂ́A�c��̂��g���̎��Ԃ�ݒ�
                tapPointDetail.PrapareteJobs(remainingTime);
            }
            
            //StartCoroutine(tapPointDetail.WorkingJobs(tapPointDetail.jobData.jobTime));
        } else {
            Debug.Log("���g���ɂ͍s���Ȃ�");
        }
    }

    /// <summary>
    /// �L��������
    /// </summary>
    public void GenerateCharaDetail(TapPointDetail tapPointDetail) {
        tapPointDetail.SwtichActivateTapPoint(false);

        // TODO ���g���p�̃L�����̐���
        Debug.Log("���g���p�̃L�����̐���");
        CharaDetail chara = Instantiate(charaDetailsPrefab, tapPointDetail.transform, false);

        chara.SetUpCharaDetail(this, tapPointDetail);
    }

    /// <summary>
    /// ���Ԃ̍������A���g�����I�����Ă��邩����
    /// </summary>
    /// <param name="jobTimeData"></param>
    /// <returns></returns>
    private (bool, int) JudgeJobsEnd(OfflineTimeManager.JobTimeData jobTimeData) {

        // �ڕW�ƂȂ鎞�Ԃ� TimeSpan �ɂ���
        //TimeSpan addTime = new TimeSpan(0, 0, tapPointDetail.jobData.jobTime, 0);

        // TODO Offline �� List �ƈ����̏����m�F

        // ���g���J�n�̎��ԂɖڕW�l�����Z����
        //DateTime resultTime = OfflineTimeManager.instance.workingJobTimeDatasList[0].GetDateTime() + addTime;

        //// ���݂̎��Ԃ��擾����
        //DateTime currentDateTime = DateTime.Now;

        //// ���g�����J�n�������ԂƁA���݂̎��Ԃ��v�Z���āA�o�߂������Ԃ̍������擾
        //TimeSpan timeElasped = currentDateTime - jobTimeData.GetDateTime();

        //// �����l�� float �^�ɕϊ�
        //int elaspedTime = (int)Math.Round(timeElasped.TotalSeconds, 0, MidpointRounding.ToEven);

        // �Q�[���N�����̎��ԂƂ��g�����J�n�������ԂƂ̍����l���Z�o
        int elaspedTime = OfflineTimeManager.instance.CalculateOfflineDateTimeElasped(jobTimeData.GetDateTime()) * 100;
        Debug.Log("���g�����Ԃ̍��� : " + elaspedTime / 100 + " : �b");

        // �c�莞�ԎZ�o
        int remainingTime = jobTimeData.elaspedJobTime;
        Debug.Log("remainingTime : " + remainingTime);

        // �o�ߎ��Ԃ����g���ɂ����鎞�Ԃ��������������Ȃ�
        if (remainingTime <= elaspedTime) {
            // ���g������
            return (true, 0);
        }
        // ���g�������B�c�莞�Ԃ���o�ߎ��Ԃ����Z���Ďc�莞�Ԃɂ���
        return (false, remainingTime - elaspedTime);
    }

    /// <summary>
    /// ���g���̐��ʔ��\
    /// �L�������^�b�v����ƌĂяo��
    /// </summary>
    public void ResultJobs(TapPointDetail tapPointDetail) {

        // TODO ����
        //Debug.Log("���� ���\");

        // ���g���̓������J�܌���
        RewardData rewardData = GetLotteryForRewards(tapPointDetail.jobData.jobType);
        Debug.Log("���肵���J�܂̒ʂ��ԍ� : " + rewardData.rewardNo);

        // �J�܃|�C���g���v�Z
        GameData.instance.CalulateTotalRewardPoint(rewardData.rewardPoint);

        // �l�������J�܂��l���σ��X�g�ɓo�^�B���łɂ���ꍇ�ɂ͏����������Z
        GameData.instance.AddEarnedRewardsList(rewardData.rewardNo);

        // �l�������J�܂̃Z�[�u
        GameData.instance.SaveEarnedReward(rewardData.rewardNo);

        // �J�܃|�C���g�̃Z�[�u
        GameData.instance.SaveTotalRewardPoint();

        // TODO �|�b�v�A�b�v�\��
        //Debug.Log("�|�b�v�A�b�v�\��");
        // ���ʃE�C���h�E����
        //RewardPopUp rewardPopUp = Instantiate(rewardPopUpPrefab, canvasTran, false);
        //rewardPopUp.SetUpRewardPopUp(rewardData);

        Instantiate(rewardPopUpPrefab, canvasTran, false).SetUpRewardPopUp(rewardData);

        // TapPoint �̏�Ԃ��ēx�������Ԃɖ߂�
        tapPointDetail.SwtichActivateTapPoint(true);

        // �摜�����̉摜�ɖ߂�
        tapPointDetail.ReturnDefaultState();

        // TODO ���g���̃��X�g�ƃZ�[�u�f�[�^���폜�@�L�������^�b�v���Ă������
        OfflineTimeManager.instance.RemoveWorkingJobTimeDatasList(tapPointDetail.jobData.jobNo);
    }

    /// <summary>
    /// ���g���̖J�܂̒��I
    /// </summary>
    private RewardData GetLotteryForRewards(JobType jobType) {
        // ��Փx�ɂ��󏭓x�̍��v�l���Z�o���āA�����_���Ȓl�𒊏o
        int randomRarityValue = UnityEngine.Random.Range(0, JobTypeRewardRatesDataSO.jobTypeRewardRatesDataList[(int)jobType].rewardRates.Sum());

        Debug.Log("����̂��g���̓�Փx : " + jobType + " / ��Փx�ɂ��󏭓x�̍��v�l : " + JobTypeRewardRatesDataSO.jobTypeRewardRatesDataList[(int)jobType].rewardRates.Sum());
        Debug.Log("�󏭓x�����肷�邽�߂̃����_���Ȓl : " + randomRarityValue);

        RarityType rarityType = RarityType.Common;
        int total = 0;

        // ���o�����l���ǂ̊󏭓x�ɂȂ邩�m�F
        for (int i = 0; i < JobTypeRewardRatesDataSO.jobTypeRewardRatesDataList.Count; i++) {
            total += JobTypeRewardRatesDataSO.jobTypeRewardRatesDataList[(int)jobType].rewardRates[i];
            Debug.Log("�󏭓x�����肷�邽�߂̃����_���Ȓl : " + randomRarityValue + " <= " + " �󏭓x�̏d�ݕt���̍��v�l : " + total);
            // 
            if (randomRarityValue <= total) {
                // 
                rarityType = (RarityType)i;
                break; 
            }
        }

        Debug.Log("����̊󏭓x : " + rarityType);

        // ����ΏۂƂȂ�󏭓x�̃f�[�^�����̃��X�g���쐬
        List<RewardData> rewardDatas = new List<RewardData>(rewardDataSO.rewardDatasList.Where(x => x.rarityType == rarityType).ToList());

        // �����󏭓x�̍��v�l���Z�o���āA�����_���Ȓl�𒊏o
        int randomRewardValue = UnityEngine.Random.Range(0, rewardDatas.Select(x => x.rarityRate).ToArray().Sum());

        Debug.Log("�󏭓x���̖J�ܗp�̃����_���Ȓl : " + randomRewardValue);

        // �ǂ̖J�܂ɂȂ邩�m�F

        total = 0;
        // ���o�����l���ǂ̖J�܂ɂȂ邩�m�F
        for (int i = 0; i < rewardDatas.Count; i++) {
            total += rewardDatas[i].rarityRate;
            Debug.Log("�󏭓x���̖J�ܗp�̃����_���Ȓl : " + randomRewardValue + " <= " + " �J�܂̏d�ݕt���̍��v�l : " + total);

            if (randomRewardValue <= total) {
                return rewardDatas[i];
            }
        }
        return null;
    }

    /// <summary>
    /// �A���o���{�^�����������ۂ̓���
    /// </summary>
    private void OnClickAlbum() {
        if (albumPopUp == null) {
            btnAlbum.transform.DOPunchScale(Vector3.one * 1.1f, 0.1f).SetEase(Ease.InOutQuart);

            albumPopUp = Instantiate(AlbumPopUpPrefab, canvasTran, false);

            //albumPopUp.transform.position = btnAlbum.transform.position;
            albumPopUp.SetUpAlbumPopUp(this, canvasTran.position, btnAlbum.transform.position);
        }
    }

    /// <summary>
    /// RewardNo ����RewardData ���擾
    /// </summary>
    /// <param name="rewardNo"></param>
    /// <returns></returns>
    public RewardData GetRewardDataFromRewardNo(int rewardNo) {
        return rewardDataSO.rewardDatasList.Find(x => x.rewardNo == rewardNo);
    }


    // mi

    /// <summary>
    /// �w�肵�����g���̔ԍ��́A���݂̎c�莞�Ԃ��擾
    /// </summary>
    /// <param name="jobNo"></param>
    /// <returns></returns>
    public int GetTapPointDetailCurrentJobTime(int jobNo) {
        return tapPointDetailsList.Find(x => x.jobData.jobNo == jobNo).GetCurrentJobTime();
    }
}
