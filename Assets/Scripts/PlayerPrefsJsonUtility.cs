using UnityEngine;

/// <summary>
/// �N���X�� Json �`���ŃZ�[�u�E���[�h
/// </summary>
public static class PlayerPrefsJsonUtility
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
    public static void SaveSetObjectData<T>(T obj, string key, bool isSave = true) {
        // �I�u�W�F�N�g�̃f�[�^�� Json �`���ɕϊ�
        string json = JsonUtility.ToJson(obj);

        // �Z�b�g
        PlayerPrefs.SetString(key, json);

        if (isSave) {
            PlayerPrefs.Save();
        }
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
    }
}
