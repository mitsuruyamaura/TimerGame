using UnityEngine;

/// <summary>
/// クラスを Json 形式でセーブ・ロード
/// </summary>
public static class PlayerPrefsJsonUtility
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
    public static void SaveSetObjectData<T>(T obj, string key, bool isSave = true) {
        // オブジェクトのデータを Json 形式に変換
        string json = JsonUtility.ToJson(obj);

        // セット
        PlayerPrefs.SetString(key, json);

        if (isSave) {
            PlayerPrefs.Save();
        }
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
    }
}
