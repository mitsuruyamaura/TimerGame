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
    /// 獲得済の褒賞の登録用クラス
    /// </summary>
    [System.Serializable]
    public class EarnedReward {
        public int rewardNo;      // 褒賞の番号 RewardData の rewardNo と照合する
        public int rewardCount;   // 褒賞の所持数
    }

    [Header("獲得している褒賞のリスト")]
    public List<EarnedReward> earnedRewardsList = new List<EarnedReward>();

    [Header("褒賞ポイントの合計値")]
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
    /// 褒賞ポイント計算
    /// </summary>
    /// <param name="amount"></param>
    public void CalulateTotalRewardPoint(int amount) {
        // 褒賞ポイントを計算して合計値算出
        //totalRewardPoint += amount;

        PointReactiveProperty.Value += amount;
    }

    /// <summary>
    /// 獲得した褒賞をリストに追加
    /// すでにリストにある場合には加算
    /// </summary>
    /// <param name="addRewardNo"></param>
    /// <param name="addRewardCount"></param>
    public void AddEarnedRewardsList(int addRewardNo, int addRewardCount = 1) {
        // すでにリストに登録がある褒賞か確認する
        if (earnedRewardsList.Exists(x => x.rewardNo == addRewardNo)) {
            // 登録がある場合には加算
            earnedRewardsList.Find(x => x.rewardNo == addRewardNo).rewardCount++;
        } else {
            // 登録がない場合には新しく追加
            earnedRewardsList.Add(new EarnedReward { rewardNo = addRewardNo, rewardCount = addRewardCount });
        }
    }

    /// <summary>
    /// 獲得している褒賞のリストを削除
    /// 番号指定ありの場合には指定された褒賞の番号の情報を削除
    /// 番号指定なしの場合にはすべて削除
    /// </summary>
    /// <param name="isAll"></param>
    public void RemoveEarnedRewardsList(int rewardNo = 999) {
        if (rewardNo == 999) {
            // すべての褒賞のデータを削除
            earnedRewardsList.Clear();
        } else {
            // 指定された褒賞の番号のデータを削除
            earnedRewardsList.Remove(earnedRewardsList.Find(x => x.rewardNo == rewardNo));
        }
    }

    /// <summary>
    /// 獲得した褒賞のデータのセーブ
    /// </summary>
    /// <param name="saveRewardNo"></param>
    public void SaveEarnedReward(int saveRewardNo) {
        PlayerPrefsHelper.SaveSetObjectData(EARNED_REWARD_SAVE_KEY + saveRewardNo, earnedRewardsList.Find(x => x.rewardNo == saveRewardNo));

    }

    /// <summary>
    /// 褒賞ポイントのセーブ
    /// </summary>
    public void SaveTotalRewardPoint() {
        //PlayerPrefsHelper.SaveIntData(TOTAL_REWARD_POINT_SAVE_KEY, totalRewardPoint);
        PlayerPrefsHelper.SaveIntData(TOTAL_REWARD_POINT_SAVE_KEY, PointReactiveProperty.Value);
    }

    /// <summary>
    /// 獲得した褒賞のデータのロード
    /// </summary>
    public void LoadEarnedRewardData() {
        for (int i = 0; i < maxRewardDataCount; i++) {
            // セーブされている褒賞のデータが存在しているか確認
            if (PlayerPrefsHelper.ExistsData(EARNED_REWARD_SAVE_KEY + i)) {
                // セーブデータがある場合のみロード
                EarnedReward earnedReward = PlayerPrefsHelper.LoadGetObjectData<EarnedReward>(EARNED_REWARD_SAVE_KEY + i);
                // リストに追加
                AddEarnedRewardsList(earnedReward.rewardNo, earnedReward.rewardCount);
            }
        }            
    }

    /// <summary>
    /// 褒賞データの最大数を登録
    /// </summary>
    /// <param name="maxCount"></param>
    public void GetMaxRewardDataCount(int maxCount) {
        maxRewardDataCount = maxCount;
    }

    /// <summary>
    /// 獲得している褒賞データの最大数の取得
    /// </summary>
    /// <returns></returns>
    public int GetEarnedRewardsListCount() {
        return earnedRewardsList.Count;
    }

    /// <summary>
    /// 褒賞ポイントのロード
    /// </summary>
    public void LoadTotalRewardPoint() {
        //totalRewardPoint = PlayerPrefsHelper.LoadIntData(TOTAL_REWARD_POINT_SAVE_KEY);
        PointReactiveProperty.Value = PlayerPrefsHelper.LoadIntData(TOTAL_REWARD_POINT_SAVE_KEY);
    }

    /// <summary>
    /// タイトルデータからキャッシュした内容の確認用
    /// </summary>
    public void SetMasterDatas() {

        jobMasterDatas = new JobData[TitleDataManager.JobMasterData.Count];
        jobMasterDatas = TitleDataManager.JobMasterData.Select(x => x.Value).ToArray();
        Debug.Log(jobMasterDatas[0].jobTime);

        jobTypeRewardRatesDatas = new JobTypeRewardRatesData[TitleDataManager.JobTypeRewardRatesMasterData.Count];
        jobTypeRewardRatesDatas = TitleDataManager.JobTypeRewardRatesMasterData.Select(x => x.Value).ToArray();
        Debug.Log(jobTypeRewardRatesDatas[0].jobType.ToString());


        // TODO　キャッシュするマスターデータが増えたら、ここに処理を追加する

    }
}
