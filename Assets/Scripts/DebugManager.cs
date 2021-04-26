using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DebugManager : MonoBehaviour
{
    public static DebugManager instance;

    [SerializeField]
    private Button btnSaveDataReset;

    [SerializeField]
    private Text txtDialog;

    [Header("デバッグを有効にする場合はチェックを入れる")]
    public bool isDebugModeOn;

    void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }

        // デバッグ機能の有効/無効化
        SetUpDebugMode();
    }

    /// <summary>
    /// デバッグ機能の有効/無効化
    /// </summary>
    private void SetUpDebugMode() {
        // ボタンを表示
        btnSaveDataReset.gameObject.SetActive(isDebugModeOn);

        // ボタンが有効なら
        if (btnSaveDataReset.gameObject.activeSelf) {
            // ボタンにメソッドを登録
            btnSaveDataReset.onClick.AddListener(OnClickAllSaveDataReset);
        }

        // ログ用のテキストを表示
        txtDialog.gameObject.SetActive(isDebugModeOn);
    }

    /// <summary>
    /// セーブデータ削除ボタンを押した際の処理
    /// </summary>
    private void OnClickAllSaveDataReset() {
        OfflineTimeManager.instance.AllRemoveWorkingJobTimeDatasList();        
    }

    /// <summary>
    /// ゲーム画面にログ表示
    /// </summary>
    /// <param name="log"></param>
    public void DisplayDebugDialog(string log) {
        txtDialog.text += log + "\n";
    }


    private void Update() {

        // デバッグ機能が無効の場合には処理を行わない
        if (!isDebugModeOn) {
            return;
        }

        // シーンの再読み込み(実行/停止の手間を省く)
        if (Input.GetKeyDown(KeyCode.Space)) {
            ReloadScene();
        }   
    }

    /// <summary>
    /// 現在のシーンを再読み込み
    /// </summary>
    private void ReloadScene() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
