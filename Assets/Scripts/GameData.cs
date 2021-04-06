using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [Header("�l�����Ă���J�܃��X�g")]
    public List<EarnedReward> earnedRewardsList = new List<EarnedReward>();

    [Header("�J�܃|�C���g")]
    public int totalRewardPoint;

    private const string EARNED_REWARD_SAVE_KEY = "earnedRewardNo_";
    private const string TOTAL_REWARD_POINT_SAVE_KEY = "totalRewardPoint";

    private int maxRewardDataCount;

    private void Awake() {
        if(instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
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
    /// �J�܃|�C���g�v�Z
    /// </summary>
    /// <param name="amount"></param>
    public void CalulateTotalRewardPoint(int amount) {
        // �J�܃|�C���g���v�Z���č��v�l�Z�o
        totalRewardPoint += amount;
    }

    /// <summary>
    /// �l�������J�܂̃f�[�^�̃Z�[�u
    /// </summary>
    /// <param name="saveRewardNo"></param>
    public void SaveEarnedReward(int saveRewardNo) {
        PlayerPrefsJsonUtility.SaveSetObjectData(earnedRewardsList.Find(x => x.rewardNo == saveRewardNo), EARNED_REWARD_SAVE_KEY + saveRewardNo);
    }

    /// <summary>
    /// �l�������J�܂̃f�[�^�̃��[�h
    /// </summary>
    public void LoadEarnedRewardData() {
        for (int i = 0; i < maxRewardDataCount; i++) {
            // �Z�[�u����Ă���J�܂̃f�[�^�����݂��Ă��邩�m�F
            if (PlayerPrefsJsonUtility.ExistsData(EARNED_REWARD_SAVE_KEY + i)) {
                // �Z�[�u�f�[�^������ꍇ�̂݃��[�h
                EarnedReward earnedReward = PlayerPrefsJsonUtility.LoadGetObjectData<EarnedReward>(EARNED_REWARD_SAVE_KEY + i);
                // ���X�g�ɒǉ�
                AddEarnedRewardsList(earnedReward.rewardNo, earnedReward.rewardCount);
            }
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
    /// �J�܃|�C���g�̃Z�[�u
    /// </summary>
    public void SaveTotalRewardPoint() {
        PlayerPrefsJsonUtility.SaveIntData(TOTAL_REWARD_POINT_SAVE_KEY, totalRewardPoint);
    }

    /// <summary>
    /// �J�܃|�C���g�̃��[�h
    /// </summary>
    public void LoadTotalRewardPoint() {
        totalRewardPoint = PlayerPrefsJsonUtility.LoadIntData(TOTAL_REWARD_POINT_SAVE_KEY);
    }
}