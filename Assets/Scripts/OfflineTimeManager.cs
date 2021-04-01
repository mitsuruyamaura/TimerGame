using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class OfflineTimeManager : MonoBehaviour
{
    public static OfflineTimeManager instance;

    private const string SAVE_KEY_STRING = "OfflineTime";
    private const string WORKING_JOB_SAVE_KEY = "workingJobNo_";

    /// <summary>
    /// お使い用の時間データを管理するためのクラス
    /// </summary>
    [Serializable]
    public class JobTimeData {
        public int jobNo;              // お使いの通し番号
        public int elespedJobTime;     // お使いの残り時間
        public string jobTimeString;   // DateTime クラスを文字列にするための変数

        /// <summary>
        /// DateTime を文字列型で保存しているので、DateTime 型に戻して取得
        /// </summary>
        /// <returns></returns>
        public DateTime GetDateTime() {
            return System.DateTime.FromBinary(System.Convert.ToInt64(jobTimeString));
        }
    }

    public List<JobTimeData> workingJobTimeDatasList = new List<JobTimeData>();

    /// <summary>
    /// 時間をセーブするための構造体(構造体は値型なので、他のクラスで利用がない場合にはクラスよりも軽く便利)
    /// </summary>
    [Serializable]
    public struct OfflineTimeData {
        public string dateTimeString;   // DateTime 型はシリアライズできないので文字列型にするために利用

        /// <summary>
        /// DateTime を文字列型で保存しているので、DateTime 型に戻して取得
        /// </summary>
        /// <returns></returns>
        public DateTime GetDateTime() {
            return System.DateTime.FromBinary(System.Convert.ToInt64(dateTimeString));
        }
    }

    public OfflineTimeData offlineTimeData;
    
    [Header("前回ゲームを止めた時にセーブしている時間")]
    public DateTime oldDateTime;

    public float ElaspedTimeInSeconds { get; set; }    // 経過時間のプロパティ

    private GameManager gameManager;


    void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }

        // セーブデータのロード
        LoadOfflineTimeData();

        // オフラインでの経過時間を計算
        CalculateOfflineEarnings();

        // TODO お使いのデータのロード
        //LoadOfflineJobTimeData(0);
    }

    /// <summary>
    /// オフラインでの経過時間を計算
    /// </summary>
    public void CalculateOfflineEarnings() {
        // 現在の時間を取得
        DateTime currentDateTime = DateTime.Now;

        // データの不正を経過時間と旧時間とでチェック
        if (oldDateTime > currentDateTime) {
            // セーブデータの時間の方が今の時間よりも進んでいる場合には、今の時間を入れなおす
            oldDateTime = DateTime.Now;
        }

        // 経過した時間の差分
        TimeSpan timeElasped = currentDateTime - oldDateTime;

        // 経過時間
        ElaspedTimeInSeconds = (int)Math.Round(timeElasped.TotalSeconds, 0, MidpointRounding.ToEven);

        Debug.Log($"オフラインでの経過時間 : {ElaspedTimeInSeconds} 秒");
    }

    /// <summary>
    /// ゲームが終了したときに自動的に呼ばれる
    /// </summary>
    private void OnApplicationQuit() {
        SaveOfflineTimeData();
        Debug.Log("ゲーム中断。時間のセーブ完了");

        // お使い中のデータがある場合、時間データをセーブ
        for (int i = 0; i < workingJobTimeDatasList.Count; i++) {
            SaveWorkingJobTimeData(workingJobTimeDatasList[i].jobNo);
        }
    }

    /// <summary>
    /// オフラインでの時間をロード
    /// </summary>
    public void LoadOfflineTimeData() {

        // セーブデータがあるか確認
        if (PlayerPrefs.HasKey(SAVE_KEY_STRING)) {
            // セーブデータがある場合
            string json = PlayerPrefs.GetString(SAVE_KEY_STRING);
            offlineTimeData = JsonUtility.FromJson<OfflineTimeData>(json);

            oldDateTime = offlineTimeData.GetDateTime();
            string str = oldDateTime.ToString("yyyy/MM/dd HH:mm:ss");
            Debug.Log($"ゲーム開始時 : セーブされていた時間 : {str}");

            str = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            Debug.Log($"今の時間 : {str}");
        } else {
            // セーブデータがない場合
            offlineTimeData = CreateOfflineTimeData();
            oldDateTime = DateTime.Now;
            string str = oldDateTime.ToString("yyyy/MM/dd HH:mm:ss");
            Debug.Log($"セーブデータがないので今の時間を取得 : {str}");
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
    public void SaveOfflineTimeData() {
        offlineTimeData.dateTimeString = DateTime.Now.ToBinary().ToString();
        
        PlayerPrefsJsonUtility.SaveSetObjectData(offlineTimeData, SAVE_KEY_STRING);

        //string json = JsonUtility.ToJson(offlineTimeData);
        //PlayerPrefs.SetString(SAVE_KEY_STRING, json);
        //PlayerPrefs.Save();

        string str = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
        Debug.Log($"ゲーム終了時 : セーブ時間 : {str}");
    }   

    /// <summary>
    /// お使いの開始時間のセーブ
    /// </summary>
    /// <param name="jobNo"></param>
    public void SaveWorkingJobTimeData(int jobNo) {

        // セーブ対象の JobTimeData を選択
        JobTimeData jobTimeData = workingJobTimeDatasList.Find(x => x.jobNo == jobNo);

        // 今の時間を設定
        jobTimeData.jobTimeString = DateTime.Now.ToBinary().ToString();

        // 現在のお使いの残り時間を設定
        jobTimeData.elespedJobTime = gameManager.GetTapPointDetailCurrentJobTime(jobNo);
        Debug.Log(jobTimeData.elespedJobTime);

        PlayerPrefsJsonUtility.SaveSetObjectData(jobTimeData, WORKING_JOB_SAVE_KEY + jobTimeData.jobNo.ToString());

        //string json = JsonUtility.ToJson(workingJobTimeDatasList[jobNo]);
        //PlayerPrefs.SetString(WORKING_JOB_SAVE_KEY + jobNo.ToString(), json);
        //PlayerPrefs.Save();

        string str = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
        Debug.Log($"仕事中 : セーブ時間 : {str}");
        Debug.Log($"セーブ時の残り時間 : {jobTimeData.elespedJobTime}");
    }

    /// <summary>
    /// お使いの開始時間のロード
    /// </summary>
    public void LoadOfflineJobTimeData(int jobNo) {
       
        // セーブデータがあるか確認
        if (PlayerPrefsJsonUtility.ExistsData(WORKING_JOB_SAVE_KEY + jobNo.ToString())) {            // PlayerPrefs.HasKey(WORKING_JOB_SAVE_KEY + jobNo.ToString())
            // セーブデータがある場合
            JobTimeData jobTimeData = PlayerPrefsJsonUtility.LoadGetObjectData<JobTimeData>(WORKING_JOB_SAVE_KEY + jobNo.ToString());
            AddWorkingJobTimeDatasList(jobTimeData);
            //string json = PlayerPrefs.GetString(WORKING_JOB_SAVE_KEY + jobNo.ToString());
            //workingJobTimeDatasList[jobNo] = JsonUtility.FromJson<JobTimeData>(json);

            DateTime time = jobTimeData.GetDateTime();
            string str =  time.ToString("yyyy/MM/dd HH:mm:ss");
            Debug.Log($"仕事開始時 : セーブされていた時間 : {str}");
            Debug.Log($"ロード時の残り時間 : {jobTimeData.elespedJobTime}");
        } 
    }

    public void SetGameManager(GameManager gameManager) {
        this.gameManager = gameManager;
    }

    /// <summary>
    /// 現在お使い中の JobTimeData の追加
    /// </summary>
    /// <param name="tapPointDetail"></param>
    public void CreateWorkingJobTimeDatasList(TapPointDetail tapPointDetail) {
        // JobTimeData を初期化
        JobTimeData jobTimeData = new JobTimeData { jobNo = tapPointDetail.jobData.jobNo, elespedJobTime = tapPointDetail.jobData.jobTime };
        AddWorkingJobTimeDatasList(jobTimeData);
    }

    /// <summary>
    /// JobTimeData を追加。このリストにある情報が現在お使いをしている内容になる
    /// </summary>
    /// <param name="jobTimeData"></param>
    public void AddWorkingJobTimeDatasList(JobTimeData jobTimeData) {
        // すでにリストにあるか確認
        if (!workingJobTimeDatasList.Exists(x => x.jobNo == jobTimeData.jobNo)) {
            workingJobTimeDatasList.Add(jobTimeData);
            Debug.Log(jobTimeData.elespedJobTime);
        }       
    }

    /// <summary>
    /// お使いの終了した JobTimeData を削除し、セーブデータを削除
    /// </summary>
    public void RemoveWorkingJobTimeDatasList(int removeJobNo) {
        // リストから削除
        workingJobTimeDatasList.Remove(workingJobTimeDatasList.Find(x => x.jobNo == removeJobNo));

        // セーブデータを削除
        PlayerPrefsJsonUtility.RemoveObjectData(WORKING_JOB_SAVE_KEY + removeJobNo);
    }
}
