using UnityEngine;

/// <summary>
/// PlayerPrefs のヘルパークラス
/// </summary>
public static class PlayerPrefsManager {

    // プロパティとして UserId を作成する
    public static string UserId
    {
        set {
            PlayerPrefs.SetString("UserId", value);
            PlayerPrefs.Save();
        }
        get => PlayerPrefs.GetString("UserId");
    }

    /// <summary>
    /// メールアドレスを利用してログイン済の場合は true
    /// </summary>
    public static bool IsLoginEmailAdress
    {

        set {
            PlayerPrefs.SetString("IsLoginEmailAdress", value.ToString());
            PlayerPrefs.Save();
        }

        get => bool.TryParse(PlayerPrefs.GetString("IsLoginEmailAdress"), out bool result) && result;
    }
}

