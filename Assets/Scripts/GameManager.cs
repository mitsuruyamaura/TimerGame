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


    void Start() {   // TODO コルーチンにする
        //OfflineTimeManager.instance.SetGameManager(this);

        // 褒賞データの最大数を登録
        GameData.instance.GetMaxRewardDataCount(rewardDataSO.rewardDatasList.Count);

        // 獲得している褒賞データの確認とロード
        GameData.instance.LoadEarnedRewardData();

        // 獲得している褒賞データがある場合
        if (GameData.instance.GetEarnedRewardsListCount() > 0) {
            // 褒賞ポイントをロード
            GameData.instance.LoadTotalRewardPoint();
        }
        
        btnAlbum.onClick.AddListener(OnClickAlbum);

        // 各 TapPointDetail に JobData を設定
        SetUpJobDatasToTapPointDetails();

        // お使いのデータのロード
        OfflineTimeManager.instance.GetWorkingJobTimeDatasList(tapPointDetailsList);

        // 各 TapPointDetail の設定
        JudgeCompleteJobs();        
    }

    /// <summary>
    /// 各 TapPointDetail に JobData を設定
    /// </summary>
    private void SetUpJobDatasToTapPointDetails() {
        for (int i = 0; i < tapPointDetailsList.Count; i++) {
            JobData jobData = jobDataSO.jobDatasList.Find(x => x.jobNo == tapPointDetailsList[i].GetMyJobNo());
            tapPointDetailsList[i].SetUpTapPointDetail(this, jobData);
        }
    }

    /// <summary>
    /// セーブされているお使いの時間データを
    /// </summary>
    private void LoadJobTimeDatas() {

    }

    /// <summary>
    /// 各 TapPointDetail のお使いの状況に合わせて、仕事中か仕事終了かを判断してキャラを生成するか、お使いを再開するか決定
    /// </summary>
    private void JudgeCompleteJobs() {
        for (int i = 0; i < tapPointDetailsList.Count; i++) {
            //JobData jobData = jobDataSO.jobDatasList.Find(x => x.jobNo == tapPointDetailsList[i].GetMyJobNo());

            //tapPointDetailsList[i].SetUpTapPointDetail(this, jobData);

            // TapPointDetail に登録されている JobData に該当する JobTimeData を取得
            OfflineTimeManager.JobTimeData jobTime = OfflineTimeManager.instance.workingJobTimeDatasList.Find((x) => x.jobNo == tapPointDetailsList[i].jobData.jobNo);

            // お使い中でなければ次の処理へ移る
            if (jobTime == null) {
                continue;
            }

            // お使いの状態と残り時間を取得
            (bool isJobEnd, int remainingTime) = JudgeJobsEnd(jobTime);
            Debug.Log(remainingTime);

            // TODO ロードしたデータを照合して、お使い中の場合には非表示にする
            if (isJobEnd) {
                // TODO お使いのリストとセーブデータを削除　キャラをタップしてから消す
                OfflineTimeManager.instance.RemoveWorkingJobTimeDatasList(jobTime.jobNo);

                // お使い終了。キャラ生成して結果を確認
                GenerateCharaDetail(tapPointDetailsList[i]);
            } else {
                // お使い再開
                JudgeSubmitJob(true, tapPointDetailsList[i], remainingTime);
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

        JobsConfirmPopUp jobsConfirmPopUp = Instantiate(jobsConfirmPopUpPrefab, canvasTran, false);

        // TODO ポップアップに JobData を送る
        jobsConfirmPopUp.OpenPopUp(tapPointDetail, this);
    }

    /// <summary>
    /// お使いを引き受けたか確認
    /// </summary>
    /// <param name="isSubmit"></param>
    public void JudgeSubmitJob(bool isSubmit, TapPointDetail tapPointDetail, int remainingTime = -1) {
        if (isSubmit) {
            // ボタンの画像を変更
            //tapPointDetail.ChangeJobSprite();

            // 仕事中の状態にする
            //tapPointDetail.IsJobs = true;

            // 仕事の登録
            OfflineTimeManager.instance.CreateWorkingJobTimeDatasList(tapPointDetail);

            // 仕事開始時間のセーブ
            OfflineTimeManager.instance.SaveWorkingJobTimeData(tapPointDetail.jobData.jobNo);

            // お使いの準備
            if (remainingTime == -1) {
                // いままでお使いしていないなら、お使いごとの初期値のお使いの時間を設定
                tapPointDetail.PrapareteJobs(tapPointDetail.jobData.jobTime);
            } else {
                // お使いの途中の場合には、残りのお使いの時間を設定
                tapPointDetail.PrapareteJobs(remainingTime);
            }
            
            //StartCoroutine(tapPointDetail.WorkingJobs(tapPointDetail.jobData.jobTime));
        } else {
            Debug.Log("お使いには行かない");
        }
    }

    /// <summary>
    /// キャラ生成
    /// </summary>
    public void GenerateCharaDetail(TapPointDetail tapPointDetail) {
        tapPointDetail.SwtichActivateTapPoint(false);

        // TODO お使い用のキャラの生成
        Debug.Log("お使い用のキャラの生成");
        CharaDetail chara = Instantiate(charaDetailsPrefab, tapPointDetail.transform, false);

        chara.SetUpCharaDetail(this, tapPointDetail);
    }

    /// <summary>
    /// 時間の差分より、お使いが終了しているか判定
    /// </summary>
    /// <param name="jobTimeData"></param>
    /// <returns></returns>
    private (bool, int) JudgeJobsEnd(OfflineTimeManager.JobTimeData jobTimeData) {

        // 目標となる時間を TimeSpan にする
        //TimeSpan addTime = new TimeSpan(0, 0, tapPointDetail.jobData.jobTime, 0);

        // TODO Offline の List と引数の情報を確認

        // お使い開始の時間に目標値を加算する
        //DateTime resultTime = OfflineTimeManager.instance.workingJobTimeDatasList[0].GetDateTime() + addTime;

        //// 現在の時間を取得する
        //DateTime currentDateTime = DateTime.Now;

        //// お使いを開始した時間と、現在の時間を計算して、経過した時間の差分を取得
        //TimeSpan timeElasped = currentDateTime - jobTimeData.GetDateTime();

        //// 差分値を float 型に変換
        //int elaspedTime = (int)Math.Round(timeElasped.TotalSeconds, 0, MidpointRounding.ToEven);

        // ゲーム起動時の時間とお使いを開始した時間との差分値を算出
        int elaspedTime = OfflineTimeManager.instance.CalculateOfflineDateTimeElasped(jobTimeData.GetDateTime()) * 100;
        Debug.Log("お使い時間の差分 : " + elaspedTime / 100 + " : 秒");

        // 残り時間算出
        int remainingTime = jobTimeData.elaspedJobTime;
        Debug.Log("remainingTime : " + remainingTime);

        // 経過時間がお使いにかかる時間よりも同じか多いなら
        if (remainingTime <= elaspedTime) {
            // お使い完了
            return (true, 0);
        }
        // お使い未了。残り時間から経過時間を減算して残り時間にする
        return (false, remainingTime - elaspedTime);
    }

    /// <summary>
    /// お使いの成果発表
    /// キャラをタップすると呼び出す
    /// </summary>
    public void ResultJobs(TapPointDetail tapPointDetail) {

        // TODO 結果
        //Debug.Log("成果 発表");

        // お使いの難しさから褒賞決定
        RewardData rewardData = GetLotteryForRewards(tapPointDetail.jobData.jobType);
        Debug.Log("決定した褒賞の通し番号 : " + rewardData.rewardNo);

        // 褒賞ポイントを計算
        GameData.instance.CalulateTotalRewardPoint(rewardData.rewardPoint);

        // 獲得した褒賞を獲得済リストに登録。すでにある場合には所持数を加算
        GameData.instance.AddEarnedRewardsList(rewardData.rewardNo);

        // 獲得した褒賞のセーブ
        GameData.instance.SaveEarnedReward(rewardData.rewardNo);

        // 褒賞ポイントのセーブ
        GameData.instance.SaveTotalRewardPoint();

        // TODO ポップアップ表示
        //Debug.Log("ポップアップ表示");
        // 成果ウインドウ生成
        //RewardPopUp rewardPopUp = Instantiate(rewardPopUpPrefab, canvasTran, false);
        //rewardPopUp.SetUpRewardPopUp(rewardData);

        Instantiate(rewardPopUpPrefab, canvasTran, false).SetUpRewardPopUp(rewardData);

        // TapPoint の状態を再度押せる状態に戻す
        tapPointDetail.SwtichActivateTapPoint(true);

        // 画像を元の画像に戻す
        tapPointDetail.ReturnDefaultState();

        // TODO お使いのリストとセーブデータを削除　キャラをタップしてから消す
        OfflineTimeManager.instance.RemoveWorkingJobTimeDatasList(tapPointDetail.jobData.jobNo);
    }

    /// <summary>
    /// お使いの褒賞の抽選
    /// </summary>
    private RewardData GetLotteryForRewards(JobType jobType) {
        // 難易度による希少度の合計値を算出して、ランダムな値を抽出
        int randomRarityValue = UnityEngine.Random.Range(0, JobTypeRewardRatesDataSO.jobTypeRewardRatesDataList[(int)jobType].rewardRates.Sum());

        Debug.Log("今回のお使いの難易度 : " + jobType + " / 難易度による希少度の合計値 : " + JobTypeRewardRatesDataSO.jobTypeRewardRatesDataList[(int)jobType].rewardRates.Sum());
        Debug.Log("希少度を決定するためのランダムな値 : " + randomRarityValue);

        RarityType rarityType = RarityType.Common;
        int total = 0;

        // 抽出した値がどの希少度になるか確認
        for (int i = 0; i < JobTypeRewardRatesDataSO.jobTypeRewardRatesDataList.Count; i++) {
            total += JobTypeRewardRatesDataSO.jobTypeRewardRatesDataList[(int)jobType].rewardRates[i];
            Debug.Log("希少度を決定するためのランダムな値 : " + randomRarityValue + " <= " + " 希少度の重み付けの合計値 : " + total);
            // 
            if (randomRarityValue <= total) {
                // 
                rarityType = (RarityType)i;
                break; 
            }
        }

        Debug.Log("今回の希少度 : " + rarityType);

        // 今回対象となる希少度のデータだけのリストを作成
        List<RewardData> rewardDatas = new List<RewardData>(rewardDataSO.rewardDatasList.Where(x => x.rarityType == rarityType).ToList());

        // 同じ希少度の合計値を算出して、ランダムな値を抽出
        int randomRewardValue = UnityEngine.Random.Range(0, rewardDatas.Select(x => x.rarityRate).ToArray().Sum());

        Debug.Log("希少度内の褒賞用のランダムな値 : " + randomRewardValue);

        // どの褒賞になるか確認

        total = 0;
        // 抽出した値がどの褒賞になるか確認
        for (int i = 0; i < rewardDatas.Count; i++) {
            total += rewardDatas[i].rarityRate;
            Debug.Log("希少度内の褒賞用のランダムな値 : " + randomRewardValue + " <= " + " 褒賞の重み付けの合計値 : " + total);

            if (randomRewardValue <= total) {
                return rewardDatas[i];
            }
        }
        return null;
    }

    /// <summary>
    /// アルバムボタンを押した際の動作
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
    /// RewardNo からRewardData を取得
    /// </summary>
    /// <param name="rewardNo"></param>
    /// <returns></returns>
    public RewardData GetRewardDataFromRewardNo(int rewardNo) {
        return rewardDataSO.rewardDatasList.Find(x => x.rewardNo == rewardNo);
    }


    // mi

    /// <summary>
    /// 指定したお使いの番号の、現在の残り時間を取得
    /// </summary>
    /// <param name="jobNo"></param>
    /// <returns></returns>
    public int GetTapPointDetailCurrentJobTime(int jobNo) {
        return tapPointDetailsList.Find(x => x.jobData.jobNo == jobNo).GetCurrentJobTime();
    }
}
