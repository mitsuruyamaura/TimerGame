using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class OfflineTimeManager : MonoBehaviour
{
    public static OfflineTimeManager instance;

    private const string SAVE_KEY_STRING = "OfflineTime";
    private const string WORKING_JOB_SAVE_KEY = "workingJobNo_";

    [Serializable]
    public class JobTimeData {
        public int jobNo;
        public string jobTimeString;

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
    /// 時間のセーブデータクラス
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
    
    [Header("前回辞めた時にセーブしている時間")]
    public DateTime oldDateTime;

    public float ElaspedTimeInSeconds { get; set; }    // 経過時間のプロパティ

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

            str = DateTime.Now.ToString("yyyy/MM/dd:mm:ss");
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
    /// セーブデータ用のクラスを作成
    /// </summary>
    private OfflineTimeData CreateOfflineTimeData() {
        OfflineTimeData offlineTimeData = new OfflineTimeData {
            dateTimeString = DateTime.Now.ToBinary().ToString(),
        };
        return offlineTimeData;
    }


    /// <summary>
    /// セーブ
    /// </summary>
    public void SaveOfflineTimeData() {
        offlineTimeData.dateTimeString = DateTime.Now.ToBinary().ToString();
        string json = JsonUtility.ToJson(offlineTimeData);

        PlayerPrefs.SetString(SAVE_KEY_STRING, json);
        PlayerPrefs.Save();

        string str = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
        Debug.Log($"ゲーム終了時 : セーブ時間 : {str}");
    }   

    public void SaveWorkingJobTimeData(int jobNo) {    

        workingJobTimeDatasList[jobNo].jobTimeString = DateTime.Now.ToBinary().ToString();
        string json = JsonUtility.ToJson(workingJobTimeDatasList[jobNo]);

        PlayerPrefs.SetString(WORKING_JOB_SAVE_KEY + jobNo.ToString(), json);
        PlayerPrefs.Save();

        string str = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
        Debug.Log($"仕事開始 : セーブ時間 : {str}");
    }
}
