using UnityEngine;

/// <summary>
/// クラスを Json 形式でセーブ・ロード
/// </summary>
public static class PlayerPrefsHelper
{
    /// <summary>
    /// 指定したキーのデータが存在しているか確認
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public static bool ExistsData(string key) {
        return PlayerPrefs.HasKey(key);
    }

    /// <summary>
    /// 指定されたオブジェクトのデータをセーブ
    /// </summary>
    /// <param name="key">データを識別するためのキー</param>
    /// <param name="isSave"></param>
    public static void SaveSetObjectData<T>(string key, T obj) {
        // オブジェクトのデータを Json 形式に変換
        string json = JsonUtility.ToJson(obj);

        // セット
        PlayerPrefs.SetString(key, json);

        // セットした Key と json をセーブ
        PlayerPrefs.Save();
    }

    /// <summary>
    /// 指定されたオブジェクトのデータをロード
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    public static T LoadGetObjectData<T>(string key) {
        // セーブされているデータをロード
        string json = PlayerPrefs.GetString(key);

        // 読み込む型を指定して変換して取得
        return JsonUtility.FromJson<T>(json);
    }

    /// <summary>
    /// 指定されたキーのデータを削除
    /// </summary>
    /// <param name="key"></param>
    public static void RemoveObjectData(string key) {
        PlayerPrefs.DeleteKey(key);
        DebugManager.instance.DisplayDebugDialog("セーブデータを削除　実行 : " + key);
        Debug.Log("セーブデータを削除　実行 : " + key);
    }

    /// <summary>
    /// すべてのセーブデータを削除
    /// </summary>
    public static void AllClearSaveData() {
        PlayerPrefs.DeleteAll();

        DebugManager.instance.DisplayDebugDialog("全セーブデータを削除　実行");
        Debug.Log("全セーブデータを削除　実行");
    }

    /// <summary>
    /// 整数データのセーブ
    /// </summary>
    /// <param name="key"></param>
    /// <param name="saveValue"></param>
    public static void SaveIntData(string key, int saveValue) {
        PlayerPrefs.SetInt(key, saveValue);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// 整数データのロード
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public static int LoadIntData(string key) {
        return PlayerPrefs.GetInt(key);
    }

    /// <summary>
    /// 文字列データのセーブ
    /// </summary>
    /// <param name="key"></param>
    /// <param name="saveValue"></param>
    public static void SaveStringData(string key, string saveValue) {
        PlayerPrefs.SetString(key, saveValue);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// 文字列データのロード
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public static string LoadStringData(string key) {
        return PlayerPrefs.GetString(key);
    }
}
