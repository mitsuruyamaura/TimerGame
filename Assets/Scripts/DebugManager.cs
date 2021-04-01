using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugManager : MonoBehaviour
{
    public static DebugManager instance;

    [SerializeField]
    private Button btnSaveDataReset;

    [SerializeField]
    private Text txtDialog;

    void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        btnSaveDataReset.onClick.AddListener(OnClickAllSaveDataReset);
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
}
