using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Cysharp.Threading.Tasks;

public class OfflineTimeManager : MonoBehaviour
{
    public static OfflineTimeManager instance;

    private DateTime loadDateTime = new DateTime();   // 前回ゲームを止めた時にセーブしている時間
    public DateTime LoadDateTime { get; set; }

    private int elaspedTime;    // 経過時間

    private GameManager gameManager;

    private const string SAVE_KEY_DATETIME = "OfflineDateTime";
    private const string FORMAT = "yyyy/MM/dd HH:mm:ss";

    //private const string SAVE_KEY_STRING = "OfflineTime";
    private const string WORKING_JOB_SAVE_KEY = "workingJobNo_";

    /// <summary>
    /// お使い用の時間データを管理するためのクラス
    /// </summary>
    [Serializable]
    public class JobTimeData {
        public int jobNo;              // お使いの通し番号
        public int elaspedJobTime;     // お使いの残り時間
        public string jobTimeString;   // DateTime クラスを文字列にするための変数

        /// <summary>
        /// DateTime を文字列型で保存しているので、DateTime 型に戻して取得
        /// </summary>
        /// <returns></returns>
        public DateTime GetDateTime() {
            return DateTime.ParseExact(jobTimeString, FORMAT, null);
            //return System.DateTime.FromBinary(System.Convert.ToInt64(jobTimeString));
        }
    }

    [Header("お使いの時間データのリスト")]
    public List<JobTimeData> workingJobTimeDatasList = new List<JobTimeData>();

    /// <summary>
    /// 時間をセーブするための構造体(構造体は値型なので、他のクラスで利用がない場合にはクラスよりも軽く便利)
    /// </summary>
    [Serializable]
    public struct OfflineTimeData {  // 不要
        public string dateTimeString;   // DateTime 型はシリアライズできないので文字列型にするために利用

        /// <summary>
        /// DateTime を文字列型で保存しているので、DateTime 型に戻して取得
        /// </summary>
        /// <returns></returns>
        public DateTime GetDateTime() {
            return System.DateTime.FromBinary(System.Convert.ToInt64(dateTimeString));
        }
    }

    [HideInInspector]
    public OfflineTimeData offlineTimeData;  // 不要


    void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }

        // セーブデータのロード
        //LoadOfflineDateTime();

        // オフラインでの経過時間を計算
        //CalculateOfflineDateTimeElasped(loadDateTime);

        // TODO お使いのデータのロード
        //LoadOfflineJobTimeData(0);
    }

    /// <summary>
    /// オフラインでの経過時間を計算
    /// </summary>
    public int CalculateOfflineDateTimeElasped(DateTime oldDateTime) {
        // 現在の時間を取得
        DateTime currentDateTime = DateTime.Now;

        // データの不正を経過時間と旧時間とでチェック
        if (oldDateTime > currentDateTime) {
            // セーブデータの時間の方が今の時間よりも進んでいる場合には、今の時間を入れなおす
            oldDateTime = DateTime.Now;
        }

        // 経過した時間の差分
        TimeSpan dateTimeElasped = currentDateTime - oldDateTime;

        // https://www.delftstack.com/ja/howto/csharp/covert-decimal-to-int-in-csharp/ cast
        // https://docs.microsoft.com/ja-jp/dotnet/api/system.midpointrounding?view=net-5.0 ToEven
        // https://docs.microsoft.com/ja-jp/dotnet/api/system.timespan.totalseconds?view=net-5.0 TotalSeconds プロパティ
        // Math.Round メソッドは 小数点の値を最も近い整数値の10進値に丸めるためのメソッド
        // 経過時間(Math.Round メソッドを利用して、double 型を int 型に変換。小数点は 0 の位で、数値の丸めの処理の指定は ToEven(数値が 2 つの数値の中間に位置するときに、最も近い偶数の値) を指定) 
        elaspedTime = (int)Math.Round(dateTimeElasped.TotalSeconds, 0, MidpointRounding.ToEven);

        Debug.Log($"オフラインでの経過時間 : {elaspedTime} 秒");

        DebugManager.instance.DisplayDebugDialog($"オフラインでの経過時間 : {elaspedTime} 秒");

        return elaspedTime;
    }

    /// <summary>
    /// ゲームが終了したときに自動的に呼ばれる
    /// </summary>
    private async UniTask OnApplicationQuit() {　　　　　　//　async UniTask に変更

        // PlayFab にゲーム終了時の時間を保存
        await OnlineTimeManager.UpdateLogOffTimeAsync();

        // PlayFab に仕事の残り時間を保存
        if (workingJobTimeDatasList.Count > 0) {
            await OnlineTimeManager.UpdateJobTimeAsync(workingJobTimeDatasList);
        }


        SaveOfflineDateTime();
        Debug.Log("ゲーム中断。時間のセーブ完了");

        DebugManager.instance.DisplayDebugDialog("ゲーム中断。時間のセーブ完了");

        

        // お使い中のデータがある場合、お使いの時間データをセーブ
        for (int i = 0; i < workingJobTimeDatasList.Count; i++) {
            SaveWorkingJobTimeData(workingJobTimeDatasList[i].jobNo);
        }
    }

    /// <summary>
    /// オフラインでの時間をロード
    /// </summary>
    public void LoadOfflineDateTime() {

        // セーブデータがあるか確認
        if (PlayerPrefsHelper.ExistsData(SAVE_KEY_DATETIME)) {

            string oldDateTimeString = PlayerPrefsHelper.LoadStringData(SAVE_KEY_DATETIME);
            //Debug.Log(oldDateTimeString);

            //if (!string.IsNullOrEmpty(oldDateTimeString))
            loadDateTime = DateTime.ParseExact(oldDateTimeString, FORMAT, null);

            // セーブデータがある場合
            //offlineTimeData = PlayerPrefsHelper.LoadGetObjectData<OfflineTimeData>(SAVE_KEY_STRING);

            //string json = PlayerPrefs.GetString(SAVE_KEY_STRING);
            //offlineTimeData = JsonUtility.FromJson<OfflineTimeData>(json);

            //oldDateTime = offlineTimeData.GetDateTime();
            //string str = oldDateTime.ToString(FORMAT);
            Debug.Log($"ゲーム開始時 : セーブされていた時間 : {oldDateTimeString}");

            //string str = DateTime.Now.ToString(FORMAT);
            Debug.Log($"今の時間 : {DateTime.Now.ToString(FORMAT)}");

            DebugManager.instance.DisplayDebugDialog($"ゲーム開始時 : セーブされていた時間 : {oldDateTimeString}");
            DebugManager.instance.DisplayDebugDialog($"今の時間 : {DateTime.Now.ToString(FORMAT)}");

        } else {
            // セーブデータがない場合、セーブデータ用の構造体を作成
            //offlineTimeData = new OfflineTimeData { dateTimeString = DateTime.Now.ToBinary().ToString() };
            loadDateTime = DateTime.Now;
            string str = loadDateTime.ToString(FORMAT);
            Debug.Log($"セーブデータがないので今の時間を取得 : {str}");

            DebugManager.instance.DisplayDebugDialog($"セーブデータがないので今の時間を取得 : {str}");
        }
    }

    /// <summary>
    /// セーブデータ用の構造体を作成
    /// </summary>
    private OfflineTimeData CreateOfflineTimeData() {
        // 構造体の場合()なしでも問題なし。合っても問題なし。
        return new OfflineTimeData { dateTimeString = DateTime.Now.ToBinary().ToString() }; 
    }

    /// <summary>
    /// 現在の時間をセーブ
    /// </summary>
    public void SaveOfflineDateTime() {
        //offlineTimeData.dateTimeString = DateTime.Now.ToBinary().ToString();

        //string dateTimeString = DateTime.Now.ToBinary().ToString();


        //PlayerPrefsHelper.SaveSetObjectData(offlineTimeData, SAVE_KEY_STRING);


        string dateTimeString = DateTime.Now.ToString(FORMAT);
        PlayerPrefsHelper.SaveStringData(SAVE_KEY_DATETIME, dateTimeString);

        //PlayerPrefs.SetString(SAVE_KEY_DATETIME, dateTimeString);
        //PlayerPrefs.Save();

        //Debug.Log(dateTimeString);

        //string json = JsonUtility.ToJson(offlineTimeData);
        //PlayerPrefs.SetString(SAVE_KEY_STRING, json);
        //PlayerPrefs.Save();

        //string str = DateTime.Now.ToString(FORMAT);
        Debug.Log($"ゲーム終了時 : セーブ時間 : {dateTimeString}");

        // ゲーム終了しているので、画面で見れないため不要
        //DebugManager.instance.DisplayDebugDialog($"ゲーム終了時 : セーブ時間 : {dateTimeString}");
    }

    /// <summary>
    ///  各お使いの残り時間の更新
    /// </summary>
    /// <param name="jobNo"></param>
    /// <param name="currentJobTime"></param>
    public void UpdateCurrentJobTime(int jobNo, int currentJobTime) {
        workingJobTimeDatasList.Find(x => x.jobNo == jobNo).elaspedJobTime = currentJobTime;
    }

    /// <summary>
    /// GameManager の情報を取得
    /// </summary>
    /// <param name="gameManager"></param>
    //public void SetGameManager(GameManager gameManager) {
    //    this.gameManager = gameManager;
    //}

    /// <summary>
    /// List に JobTimeData を追加。このリストにある情報が現在お使いをしている内容になる
    /// </summary>
    /// <param name="jobTimeData"></param>
    public void AddWorkingJobTimeDatasList(JobTimeData jobTimeData) {
        // すでにリストにあるか確認
        if (!workingJobTimeDatasList.Exists(x => x.jobNo == jobTimeData.jobNo)) {
            workingJobTimeDatasList.Add(jobTimeData);
            Debug.Log(jobTimeData.elaspedJobTime);
        }
    }

    /// <summary>
    /// 現在お使い中の JobTimeData の作成と List への追加
    /// </summary>
    /// <param name="tapPointDetail"></param>
    public void CreateWorkingJobTimeDatasList(TapPointDetail tapPointDetail) {
        // JobTimeData を初期化
        JobTimeData jobTimeData = new JobTimeData { jobNo = tapPointDetail.jobData.jobNo, elaspedJobTime = tapPointDetail.jobData.jobTime };
        AddWorkingJobTimeDatasList(jobTimeData);
    }

    /// <summary>
    /// お使いの時間のセーブ
    /// お使い開始時とゲーム終了時にセーブ
    /// </summary>
    /// <param name="jobNo"></param>
    public void SaveWorkingJobTimeData(int jobNo) {

        // セーブ対象の JobTimeData を設定
        JobTimeData jobTimeData = workingJobTimeDatasList.Find(x => x.jobNo == jobNo);

        // 今の時間を取得
        //jobTimeData.jobTimeString = DateTime.Now.ToBinary().ToString();

        jobTimeData.jobTimeString = DateTime.Now.ToString(FORMAT);

        // 現在のお使いの残り時間を取得
        //jobTimeData.elaspedJobTime = gameManager.GetTapPointDetailCurrentJobTime(jobNo);
        //Debug.Log(jobTimeData.elespedJobTime);

        PlayerPrefsHelper.SaveSetObjectData(WORKING_JOB_SAVE_KEY + jobTimeData.jobNo.ToString(), jobTimeData);

        //string json = JsonUtility.ToJson(workingJobTimeDatasList[jobNo]);
        //PlayerPrefs.SetString(WORKING_JOB_SAVE_KEY + jobNo.ToString(), json);
        //PlayerPrefs.Save();

        string str = DateTime.Now.ToString(FORMAT);
        Debug.Log($"仕事中 : セーブ時間 : {str}");
        Debug.Log($"セーブ時のお使いの残り時間 : {jobTimeData.elaspedJobTime}");

        DebugManager.instance.DisplayDebugDialog($"仕事中 : セーブ時間 : {str}");
        DebugManager.instance.DisplayDebugDialog($"セーブ時の残り時間 : {jobTimeData.elaspedJobTime}");
    }

    /// <summary>
    /// 行き先の数だけ、その行き先の JobTimeData があるかどうか確認し、ある場合にはロードして WorkingJobTimeDatasList に追加
    /// </summary>
    public void GetWorkingJobTimeDatasList(List<TapPointDetail> tapPointDetailsList) {
        for (int i = 0; i < tapPointDetailsList.Count; i++) {
            // 該当するお使いの番号でセーブされている時間データがあるかどうか確認
            LoadOfflineJobTimeData(tapPointDetailsList[i].jobData.jobNo);
        }

        Debug.Log("お使いのデータを取得");
    }

    /// <summary>
    /// お使いの開始時間のロード
    /// </summary>
    /// <param name="jobNo"></param>
    public void LoadOfflineJobTimeData(int jobNo) {

        // セーブデータがあるか確認
        if (PlayerPrefsHelper.ExistsData(WORKING_JOB_SAVE_KEY + jobNo.ToString())) {            // PlayerPrefs.HasKey(WORKING_JOB_SAVE_KEY + jobNo.ToString())
            // セーブデータがある場合
            JobTimeData jobTimeData = PlayerPrefsHelper.LoadGetObjectData<JobTimeData>(WORKING_JOB_SAVE_KEY + jobNo.ToString());
            AddWorkingJobTimeDatasList(jobTimeData);
            //string json = PlayerPrefs.GetString(WORKING_JOB_SAVE_KEY + jobNo.ToString());
            //workingJobTimeDatasList[jobNo] = JsonUtility.FromJson<JobTimeData>(json);

            DateTime time = jobTimeData.GetDateTime();

            string str = time.ToString(FORMAT);
            Debug.Log($"仕事開始時 : セーブされていた時間 : {str}");
            Debug.Log($"ロード時の残り時間 : {jobTimeData.elaspedJobTime}");

            DebugManager.instance.DisplayDebugDialog($"仕事開始時 : セーブされていた時間 : {str}");
            DebugManager.instance.DisplayDebugDialog($"ロード時の残り時間 : {jobTimeData.elaspedJobTime}");
        }
    }

    /// <summary>
    /// お使いの終了した JobTimeData を削除し、セーブデータを削除
    /// </summary>
    public async void RemoveWorkingJobTimeDatasList(int removeJobNo) {
        // リストから削除
        workingJobTimeDatasList.Remove(workingJobTimeDatasList.Find(x => x.jobNo == removeJobNo));

        // セーブデータを削除
        PlayerPrefsHelper.RemoveObjectData(WORKING_JOB_SAVE_KEY + removeJobNo);

        // PlayFab のお使いのデータを更新
        await OnlineTimeManager.UpdateJobTimeAsync(workingJobTimeDatasList);
    }

    /// <summary>
    /// デバッグ用
    /// すべてのお使いの JobTimeData を削除
    /// </summary>
    public void AllRemoveWorkingJobTimeDatasList() {
        // リストからすべて削除
        workingJobTimeDatasList.Clear();

        // すべてのセーブデータを削除
        PlayerPrefsHelper.AllClearSaveData();
        DebugManager.instance.DisplayDebugDialog("すべてのセーブデータを削除 実行");
    }
}
