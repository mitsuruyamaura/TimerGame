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


    void Start() {   // TODO コルーチンにする
        OfflineTimeManager.instance.SetGameManager(this);

        // TODO お使いのデータのロード
        OfflineTimeManager.instance.LoadOfflineJobTimeData(0);

        // TODO キャラの生成確認

        TapPointSetUp();   
    }

    /// <summary>
    /// 各 TapPointDetail の設定。お使いの状況に合わせて、仕事中か仕事終了かを判断してキャラを生成するか、お使いを再開するか決定
    /// </summary>
    private void TapPointSetUp() {
        for (int i = 0; i < tapPointDetailsList.Count; i++) {
            tapPointDetailsList[i].SetUpTapPointDetail(this);

            // TapPointDetail に登録されている JobData に該当する JobTimeData を取得
            OfflineTimeManager.JobTimeData jobTime = OfflineTimeManager.instance.workingJobTimeDatasList.Find((x) => x.jobNo == tapPointDetailsList[i].jobData.jobNo);

            // お使い中でなければ次の処理へ移る
            if (jobTime == null) {
                continue;
            }

            // TODO ロードしたデータを照合して、お使い中の場合には非表示にする
            if (JudgeJobsEnd(jobTime)) {
                // TODO お使いのリストとセーブデータを削除　キャラをタップしてから消す
                //OfflineTimeManager.instance.RemoveWorkingJobTimeDatasList(jobTime.jobNo);

                // お使い終了。キャラ生成して結果を確認
                GenerateChara(tapPointDetailsList[i]);
            } else {
                // お使い再開
                JudgeSubmitJob(true, tapPointDetailsList[i]);
            }
        }
    }

    /// <summary>
    /// TapPoint をクリックした際にお使い確認用のポップアップを開く
    /// </summary>
    /// <param name="tapPointDetail"></param>
    public void GenerateJobsConfirmPopUp(TapPointDetail tapPointDetail) {

        // TODO ポップアップをインスタンスする 
        Debug.Log("お使い確認用のポップアップを開く");

        jobsConfirmPopUp = Instantiate(jobsConfirmPopUpPrefab, canvasTran, false);

        // TODO ポップアップに JobData を送る
        jobsConfirmPopUp.OpenPopUp(this, tapPointDetail);
    }

    /// <summary>
    /// お使いを引き受けたか確認
    /// </summary>
    /// <param name="isSubmit"></param>
    public void JudgeSubmitJob(bool isSubmit, TapPointDetail tapPointDetail) {
        if (isSubmit) {
            // ボタンの画像を変更
            tapPointDetail.ChangeJobSprite();

            // 仕事中の状態にする
            tapPointDetail.isJobs = true;

            // 仕事の登録
            OfflineTimeManager.instance.CreateWorkingJobTimeDatasList(tapPointDetail);

            // 仕事開始時間のセーブ
            OfflineTimeManager.instance.SaveWorkingJobTimeData(tapPointDetail.jobData.jobNo);

            // 仕事開始
            StartCoroutine(tapPointDetail.WorkingJobs(tapPointDetail.jobData.jobTime));
        } else {
            Debug.Log("お使いには行かない");
        }
    }

    /// <summary>
    /// キャラ生成
    /// </summary>
    public void GenerateChara(TapPointDetail tapPointDetail) {
        tapPointDetail.SwtichActivateTapPoint(false);

        // TODO お使い用のキャラの生成
        Debug.Log("お使い用のキャラの生成");
        CharaController chara = Instantiate(charaPrefab, tapPointDetail.transform, false);

        chara.SetUpChara(this, tapPointDetail);
    }

    /// <summary>
    /// 時間の差分より、お使いが終了しているか判定
    /// </summary>
    /// <param name="jobTimeData"></param>
    /// <returns></returns>
    private bool JudgeJobsEnd(OfflineTimeManager.JobTimeData jobTimeData) {

        // 目標となる時間を TimeSpan にする
        //TimeSpan addTime = new TimeSpan(0, 0, tapPointDetail.jobData.jobTime, 0);

        // TODO Offline の List と引数の情報を確認

        // お使い開始の時間に目標値を加算する
        //DateTime resultTime = OfflineTimeManager.instance.workingJobTimeDatasList[0].GetDateTime() + addTime;

        // 現在の時間を取得する
        DateTime currentDateTime = DateTime.Now;

        // お使いを開始した時間と、現在の時間を計算して、経過した時間の差分を取得
        TimeSpan timeElasped = currentDateTime - jobTimeData.GetDateTime();

        // 差分値を float 型に変換
        float elapsedTime = (int)Math.Round(timeElasped.TotalSeconds, 0, MidpointRounding.ToEven);
        Debug.Log("お使い時間の差分 : " + elapsedTime + " : 秒");

        // 経過時間がお使いにかかる時間よりも同じか多いなら
        if ((float)jobTimeData.elespedJobTime <= elapsedTime) {
            return true;
        }

        return false;
    }

    /// <summary>
    /// 指定したお使いの番号の、現在の残り時間を取得
    /// </summary>
    /// <param name="jobNo"></param>
    /// <returns></returns>
    public int GetTapPointDetailCurrentJobTime(int jobNo) {
        return tapPointDetailsList.Find(x => x.jobData.jobNo == jobNo).GetCurrentJobTime();
    }

    /// <summary>
    /// お使いの成果発表
    /// キャラをタップすると呼び出す
    /// </summary>
    public void ResultJobs(TapPointDetail tapPointDetail) {

        // TODO 結果
        Debug.Log("成果 発表");

        // お使いの難しさから褒賞決定
        RewardData rewardData = GetLotteryForRrewards(tapPointDetail.jobData.jobType);
        Debug.Log(rewardData.rewardNo);

        // TODO ポップアップ表示
        Debug.Log("ポップアップ表示");
        // 成果ウインドウ生成
        //RewardPopUp rewardPopUp = Instantiate(rewardPopUpPrefab, canvasTran, false);
        //rewardPopUp.SetUpRewardPopUp(rewardData);

        // TapPoint の状態を再度押せる状態に戻す
        tapPointDetail.SwtichActivateTapPoint(true);

        // 画像を元の画像に戻す
        tapPointDetail.ChangeDefaultSprite();

        // TODO お使いのリストとセーブデータを削除　キャラをタップしてから消す
        OfflineTimeManager.instance.RemoveWorkingJobTimeDatasList(tapPointDetail.jobData.jobNo);
    }

    /// <summary>
    /// お使いのご褒美の抽選
    /// </summary>
    private RewardData GetLotteryForRrewards(JobType jobType) {
        // 難易度による希少度の合計値を算出して、ランダムな値を抽出
        int randomRarityValue = UnityEngine.Random.Range(0, JobTypeRewardRatesDataSO.jobTypeRewardRatesDataList[(int)jobType].rewardRates.Sum());

        Debug.Log(JobTypeRewardRatesDataSO.jobTypeRewardRatesDataList[(int)jobType].rewardRates.Sum());

        RarityType rarityType = RarityType.Common;
        int total = 0;
        Debug.Log(randomRarityValue);

        // 抽出した値がどの希少度になるか確認
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

        // 今回対象となる希少度のデータだけのリストを作成
        List<RewardData> rewardDatas = new List<RewardData>(rewardDataSO.rewardDatasList.Where(x => x.rarityType == rarityType).ToList());

        // 同じ希少度の合計値を算出して、ランダムな値を抽出
        int randomRewardValue = UnityEngine.Random.Range(0, rewardDatas.Select(x => x.rarityRate).ToArray().Sum());

        Debug.Log(randomRewardValue);

        // どの褒賞になるか確認

        total = 0;
        // 抽出した値がどの褒賞になるか確認
        for (int i = 0; i < rewardDatas.Count; i++) {
            total += rewardDatas[i].rarityRate;

            if (randomRewardValue <= total) {
                return rewardDatas[i];
            }
        }
        return null;
    }
}
