using UnityEngine;

/// <summary>
/// �N���X�� Json �`���ŃZ�[�u�E���[�h
/// </summary>
public static class PlayerPrefsHelper
{
    /// <summary>
    /// �w�肵���L�[�̃f�[�^�����݂��Ă��邩�m�F
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public static bool ExistsData(string key) {
        return PlayerPrefs.HasKey(key);
    }

    /// <summary>
    /// �w�肳�ꂽ�I�u�W�F�N�g�̃f�[�^���Z�[�u
    /// </summary>
    /// <param name="key">�f�[�^�����ʂ��邽�߂̃L�[</param>
    /// <param name="isSave"></param>
    public static void SaveSetObjectData<T>(string key, T obj) {
        // �I�u�W�F�N�g�̃f�[�^�� Json �`���ɕϊ�
        string json = JsonUtility.ToJson(obj);

        // �Z�b�g
        PlayerPrefs.SetString(key, json);

        // �Z�b�g���� Key �� json ���Z�[�u
        PlayerPrefs.Save();
    }

    /// <summary>
    /// �w�肳�ꂽ�I�u�W�F�N�g�̃f�[�^�����[�h
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    public static T LoadGetObjectData<T>(string key) {
        // �Z�[�u����Ă���f�[�^�����[�h
        string json = PlayerPrefs.GetString(key);

        // �ǂݍ��ތ^���w�肵�ĕϊ����Ď擾
        return JsonUtility.FromJson<T>(json);
    }

    /// <summary>
    /// �w�肳�ꂽ�L�[�̃f�[�^���폜
    /// </summary>
    /// <param name="key"></param>
    public static void RemoveObjectData(string key) {
        PlayerPrefs.DeleteKey(key);
        DebugManager.instance.DisplayDebugDialog("�Z�[�u�f�[�^���폜�@���s : " + key);
        Debug.Log("�Z�[�u�f�[�^���폜�@���s : " + key);
    }

    /// <summary>
    /// ���ׂẴZ�[�u�f�[�^���폜
    /// </summary>
    public static void AllClearSaveData() {
        PlayerPrefs.DeleteAll();

        DebugManager.instance.DisplayDebugDialog("�S�Z�[�u�f�[�^���폜�@���s");
        Debug.Log("�S�Z�[�u�f�[�^���폜�@���s");
    }

    /// <summary>
    /// �����f�[�^�̃Z�[�u
    /// </summary>
    /// <param name="key"></param>
    /// <param name="saveValue"></param>
    public static void SaveIntData(string key, int saveValue) {
        PlayerPrefs.SetInt(key, saveValue);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// �����f�[�^�̃��[�h
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public static int LoadIntData(string key) {
        return PlayerPrefs.GetInt(key);
    }

    /// <summary>
    /// ������f�[�^�̃Z�[�u
    /// </summary>
    /// <param name="key"></param>
    /// <param name="saveValue"></param>
    public static void SaveStringData(string key, string saveValue) {
        PlayerPrefs.SetString(key, saveValue);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// ������f�[�^�̃��[�h
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public static string LoadStringData(string key) {
        return PlayerPrefs.GetString(key);
    }
}
