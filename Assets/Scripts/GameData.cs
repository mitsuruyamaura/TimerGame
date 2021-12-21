using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using System.Linq;

public class GameData : MonoBehaviour
{
    public static GameData instance;

    /// <summary>
    /// �l���ς̖J�܂̓o�^�p�N���X
    /// </summary>
    [System.Serializable]
    public class EarnedReward {
        public int rewardNo;      // �J�܂̔ԍ� RewardData �� rewardNo �Əƍ�����
        public int rewardCount;   // �J�܂̏�����
    }

    [Header("�l�����Ă���J�܂̃��X�g")]
    public List<EarnedReward> earnedRewardsList = new List<EarnedReward>();

    [Header("�J�܃|�C���g�̍��v�l")]
    public int totalRewardPoint;

    private const string EARNED_REWARD_SAVE_KEY = "earnedRewardNo_";
    private const string TOTAL_REWARD_POINT_SAVE_KEY = "totalRewardPoint";

    private int maxRewardDataCount;

    public ReactiveProperty<int> PointReactiveProperty = new ReactiveProperty<int>(0);


    public JobData[] jobMasterDatas;

    public JobTypeRewardRatesData[] jobTypeRewardRatesDatas;


    private void Awake() {
        if(instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// �J�܃|�C���g�v�Z
    /// </summary>
    /// <param name="amount"></param>
    public void CalulateTotalRewardPoint(int amount) {
        // �J�܃|�C���g���v�Z���č��v�l�Z�o
        //totalRewardPoint += amount;

        PointReactiveProperty.Value += amount;
    }

    /// <summary>
    /// �l�������J�܂����X�g�ɒǉ�
    /// ���łɃ��X�g�ɂ���ꍇ�ɂ͉��Z
    /// </summary>
    /// <param name="addRewardNo"></param>
    /// <param name="addRewardCount"></param>
    public void AddEarnedRewardsList(int addRewardNo, int addRewardCount = 1) {
        // ���łɃ��X�g�ɓo�^������J�܂��m�F����
        if (earnedRewardsList.Exists(x => x.rewardNo == addRewardNo)) {
            // �o�^������ꍇ�ɂ͉��Z
            earnedRewardsList.Find(x => x.rewardNo == addRewardNo).rewardCount++;
        } else {
            // �o�^���Ȃ��ꍇ�ɂ͐V�����ǉ�
            earnedRewardsList.Add(new EarnedReward { rewardNo = addRewardNo, rewardCount = addRewardCount });
        }
    }

    /// <summary>
    /// �l�����Ă���J�܂̃��X�g���폜
    /// �ԍ��w�肠��̏ꍇ�ɂ͎w�肳�ꂽ�J�܂̔ԍ��̏����폜
    /// �ԍ��w��Ȃ��̏ꍇ�ɂ͂��ׂč폜
    /// </summary>
    /// <param name="isAll"></param>
    public void RemoveEarnedRewardsList(int rewardNo = 999) {
        if (rewardNo == 999) {
            // ���ׂĂ̖J�܂̃f�[�^���폜
            earnedRewardsList.Clear();
        } else {
            // �w�肳�ꂽ�J�܂̔ԍ��̃f�[�^���폜
            earnedRewardsList.Remove(earnedRewardsList.Find(x => x.rewardNo == rewardNo));
        }
    }

    /// <summary>
    /// �l�������J�܂̃f�[�^�̃Z�[�u
    /// </summary>
    /// <param name="saveRewardNo"></param>
    public void SaveEarnedReward(int saveRewardNo) {
        PlayerPrefsHelper.SaveSetObjectData(EARNED_REWARD_SAVE_KEY + saveRewardNo, earnedRewardsList.Find(x => x.rewardNo == saveRewardNo));

    }

    /// <summary>
    /// �J�܃|�C���g�̃Z�[�u
    /// </summary>
    public void SaveTotalRewardPoint() {
        //PlayerPrefsHelper.SaveIntData(TOTAL_REWARD_POINT_SAVE_KEY, totalRewardPoint);
        PlayerPrefsHelper.SaveIntData(TOTAL_REWARD_POINT_SAVE_KEY, PointReactiveProperty.Value);
    }

    /// <summary>
    /// �l�������J�܂̃f�[�^�̃��[�h
    /// </summary>
    public void LoadEarnedRewardData() {
        for (int i = 0; i < maxRewardDataCount; i++) {
            // �Z�[�u����Ă���J�܂̃f�[�^�����݂��Ă��邩�m�F
            if (PlayerPrefsHelper.ExistsData(EARNED_REWARD_SAVE_KEY + i)) {
                // �Z�[�u�f�[�^������ꍇ�̂݃��[�h
                EarnedReward earnedReward = PlayerPrefsHelper.LoadGetObjectData<EarnedReward>(EARNED_REWARD_SAVE_KEY + i);
                // ���X�g�ɒǉ�
                AddEarnedRewardsList(earnedReward.rewardNo, earnedReward.rewardCount);
            }
        }            
    }

    /// <summary>
    /// �J�܃f�[�^�̍ő吔��o�^
    /// </summary>
    /// <param name="maxCount"></param>
    public void GetMaxRewardDataCount(int maxCount) {
        maxRewardDataCount = maxCount;
    }

    /// <summary>
    /// �l�����Ă���J�܃f�[�^�̍ő吔�̎擾
    /// </summary>
    /// <returns></returns>
    public int GetEarnedRewardsListCount() {
        return earnedRewardsList.Count;
    }

    /// <summary>
    /// �J�܃|�C���g�̃��[�h
    /// </summary>
    public void LoadTotalRewardPoint() {
        //totalRewardPoint = PlayerPrefsHelper.LoadIntData(TOTAL_REWARD_POINT_SAVE_KEY);
        PointReactiveProperty.Value = PlayerPrefsHelper.LoadIntData(TOTAL_REWARD_POINT_SAVE_KEY);
    }

    /// <summary>
    /// �^�C�g���f�[�^����L���b�V���������e�̊m�F�p
    /// </summary>
    public void SetMasterDatas() {

        jobMasterDatas = new JobData[TitleDataManager.JobMasterData.Count];
        jobMasterDatas = TitleDataManager.JobMasterData.Select(x => x.Value).ToArray();
        Debug.Log(jobMasterDatas[0].jobTime);

        jobTypeRewardRatesDatas = new JobTypeRewardRatesData[TitleDataManager.JobTypeRewardRatesMasterData.Count];
        jobTypeRewardRatesDatas = TitleDataManager.JobTypeRewardRatesMasterData.Select(x => x.Value).ToArray();
        Debug.Log(jobTypeRewardRatesDatas[0].jobType.ToString());


        // TODO�@�L���b�V������}�X�^�[�f�[�^����������A�����ɏ�����ǉ�����

    }
}
