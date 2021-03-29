using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class OfflineTimeManager : MonoBehaviour
{
    public static OfflineTimeManager instance;

    private const string SEVE_KEY_STRING = "OfflineTime";

    /// <summary>
    /// ���Ԃ̃Z�[�u�f�[�^�N���X
    /// </summary>
    [Serializable]
    public struct OfflineTimeData {
        public string dateTimeString;   // DateTime �^�̓V���A���C�Y�ł��Ȃ��̂ŕ�����^�ɂ��邽�߂ɗ��p

        /// <summary>
        /// DateTime �𕶎���^�ŕۑ����Ă���̂ŁADateTime �^�ɖ߂��Ď擾
        /// </summary>
        /// <returns></returns>
        public DateTime GetDateTime() {
            return System.DateTime.FromBinary(System.Convert.ToInt64(dateTimeString));
        }
    }

    public OfflineTimeData offlineTimeData;
    
    [Header("�O�񎫂߂����ɃZ�[�u���Ă��鎞��")]
    public DateTime oldDateTime;

    public float ElaspedTimeInSeconds { get; set; }    // �o�ߎ��Ԃ̃v���p�e�B

    void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }

        // �Z�[�u�f�[�^�̃��[�h
        LoadOfflineTimeData();

        // �I�t���C���ł̌o�ߎ��Ԃ��v�Z
        CalculateOfflineEarnings();
    }

    /// <summary>
    /// �I�t���C���ł̌o�ߎ��Ԃ��v�Z
    /// </summary>
    public void CalculateOfflineEarnings() {
        // ���݂̎��Ԃ��擾
        DateTime currentDateTime = DateTime.Now;

        // �f�[�^�̕s�����o�ߎ��ԂƋ����ԂƂŃ`�F�b�N
        if (oldDateTime > currentDateTime) {
            // �Z�[�u�f�[�^�̎��Ԃ̕������̎��Ԃ����i��ł���ꍇ�ɂ́A���̎��Ԃ����Ȃ���
            oldDateTime = DateTime.Now;
        }

        // �o�߂������Ԃ̍���
        TimeSpan timeElasped = currentDateTime - oldDateTime;

        // �o�ߎ���
        ElaspedTimeInSeconds = (int)Math.Round(timeElasped.TotalSeconds, 0, MidpointRounding.ToEven);

        Debug.Log($"�I�t���C���ł̌o�ߎ��� : {ElaspedTimeInSeconds} �b");
    }

    /// <summary>
    /// �Q�[�����I�������Ƃ��Ɏ����I�ɌĂ΂��
    /// </summary>
    private void OnApplicationQuit() {
        SaveOfflineTimeData();
        Debug.Log("�Q�[�����f�B���Ԃ̃Z�[�u����");
    }

    /// <summary>
    /// �I�t���C���ł̎��Ԃ����[�h
    /// </summary>
    public void LoadOfflineTimeData() {

        // �Z�[�u�f�[�^�����邩�m�F
        if (PlayerPrefs.HasKey(SEVE_KEY_STRING)) {
            // �Z�[�u�f�[�^������ꍇ
            string json = PlayerPrefs.GetString(SEVE_KEY_STRING);
            offlineTimeData = JsonUtility.FromJson<OfflineTimeData>(json);

            oldDateTime = offlineTimeData.GetDateTime();
            string str = oldDateTime.ToString("yyyy/MM/dd HH:mm:ss");
            Debug.Log($"�Q�[���J�n�� : �Z�[�u����Ă������� : {str}");

            str = DateTime.Now.ToString("yyyy/MM/dd:mm:ss");
            Debug.Log($"���̎��� : {str}");
        } else {
            // �Z�[�u�f�[�^���Ȃ��ꍇ
            offlineTimeData = CreateOfflineTimeData();
            oldDateTime = DateTime.Now;
            string str = oldDateTime.ToString("yyyy/MM/dd HH:mm:ss");
            Debug.Log($"�Z�[�u�f�[�^���Ȃ��̂ō��̎��Ԃ��擾 : {str}");
        }
    }

    /// <summary>
    /// �Z�[�u�f�[�^�p�̃N���X���쐬
    /// </summary>
    private OfflineTimeData CreateOfflineTimeData() {
        OfflineTimeData offlineTimeData = new OfflineTimeData {
            dateTimeString = DateTime.Now.ToBinary().ToString(),
        };
        return offlineTimeData;
    }


    /// <summary>
    /// �Z�[�u
    /// </summary>
    public void SaveOfflineTimeData() {
        string str = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

        offlineTimeData.dateTimeString = DateTime.Now.ToBinary().ToString();
        string json = JsonUtility.ToJson(offlineTimeData);

        PlayerPrefs.SetString(SEVE_KEY_STRING, json);
        PlayerPrefs.Save();

        Debug.Log($"�Q�[���I���� : �Z�[�u���� : {str}");
    }   
}
