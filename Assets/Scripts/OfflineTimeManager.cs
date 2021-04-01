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
    /// ���g���p�̎��ԃf�[�^���Ǘ����邽�߂̃N���X
    /// </summary>
    [Serializable]
    public class JobTimeData {
        public int jobNo;              // ���g���̒ʂ��ԍ�
        public int elespedJobTime;     // ���g���̎c�莞��
        public string jobTimeString;   // DateTime �N���X�𕶎���ɂ��邽�߂̕ϐ�

        /// <summary>
        /// DateTime �𕶎���^�ŕۑ����Ă���̂ŁADateTime �^�ɖ߂��Ď擾
        /// </summary>
        /// <returns></returns>
        public DateTime GetDateTime() {
            return System.DateTime.FromBinary(System.Convert.ToInt64(jobTimeString));
        }
    }

    public List<JobTimeData> workingJobTimeDatasList = new List<JobTimeData>();

    /// <summary>
    /// ���Ԃ��Z�[�u���邽�߂̍\����(�\���̂͒l�^�Ȃ̂ŁA���̃N���X�ŗ��p���Ȃ��ꍇ�ɂ̓N���X�����y���֗�)
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
    
    [Header("�O��Q�[�����~�߂����ɃZ�[�u���Ă��鎞��")]
    public DateTime oldDateTime;

    public float ElaspedTimeInSeconds { get; set; }    // �o�ߎ��Ԃ̃v���p�e�B

    private GameManager gameManager;


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

        // TODO ���g���̃f�[�^�̃��[�h
        //LoadOfflineJobTimeData(0);
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

        // ���g�����̃f�[�^������ꍇ�A���ԃf�[�^���Z�[�u
        for (int i = 0; i < workingJobTimeDatasList.Count; i++) {
            SaveWorkingJobTimeData(workingJobTimeDatasList[i].jobNo);
        }
    }

    /// <summary>
    /// �I�t���C���ł̎��Ԃ����[�h
    /// </summary>
    public void LoadOfflineTimeData() {

        // �Z�[�u�f�[�^�����邩�m�F
        if (PlayerPrefs.HasKey(SAVE_KEY_STRING)) {
            // �Z�[�u�f�[�^������ꍇ
            string json = PlayerPrefs.GetString(SAVE_KEY_STRING);
            offlineTimeData = JsonUtility.FromJson<OfflineTimeData>(json);

            oldDateTime = offlineTimeData.GetDateTime();
            string str = oldDateTime.ToString("yyyy/MM/dd HH:mm:ss");
            Debug.Log($"�Q�[���J�n�� : �Z�[�u����Ă������� : {str}");

            str = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
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
    /// �Z�[�u�f�[�^�p�̍\���̂��쐬
    /// </summary>
    private OfflineTimeData CreateOfflineTimeData() {
        // �\���̂̏ꍇ()�Ȃ��ł����Ȃ��B�����Ă����Ȃ��B
        return new OfflineTimeData { dateTimeString = DateTime.Now.ToBinary().ToString() }; 
    }

    /// <summary>
    /// ���݂̎��Ԃ��Z�[�u
    /// </summary>
    public void SaveOfflineTimeData() {
        offlineTimeData.dateTimeString = DateTime.Now.ToBinary().ToString();
        
        PlayerPrefsJsonUtility.SaveSetObjectData(offlineTimeData, SAVE_KEY_STRING);

        //string json = JsonUtility.ToJson(offlineTimeData);
        //PlayerPrefs.SetString(SAVE_KEY_STRING, json);
        //PlayerPrefs.Save();

        string str = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
        Debug.Log($"�Q�[���I���� : �Z�[�u���� : {str}");
    }   

    /// <summary>
    /// ���g���̊J�n���Ԃ̃Z�[�u
    /// </summary>
    /// <param name="jobNo"></param>
    public void SaveWorkingJobTimeData(int jobNo) {

        // �Z�[�u�Ώۂ� JobTimeData ��I��
        JobTimeData jobTimeData = workingJobTimeDatasList.Find(x => x.jobNo == jobNo);

        // ���̎��Ԃ�ݒ�
        jobTimeData.jobTimeString = DateTime.Now.ToBinary().ToString();

        // ���݂̂��g���̎c�莞�Ԃ�ݒ�
        jobTimeData.elespedJobTime = gameManager.GetTapPointDetailCurrentJobTime(jobNo);
        Debug.Log(jobTimeData.elespedJobTime);

        PlayerPrefsJsonUtility.SaveSetObjectData(jobTimeData, WORKING_JOB_SAVE_KEY + jobTimeData.jobNo.ToString());

        //string json = JsonUtility.ToJson(workingJobTimeDatasList[jobNo]);
        //PlayerPrefs.SetString(WORKING_JOB_SAVE_KEY + jobNo.ToString(), json);
        //PlayerPrefs.Save();

        string str = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
        Debug.Log($"�d���� : �Z�[�u���� : {str}");
        Debug.Log($"�Z�[�u���̎c�莞�� : {jobTimeData.elespedJobTime}");
    }

    /// <summary>
    /// ���g���̊J�n���Ԃ̃��[�h
    /// </summary>
    public void LoadOfflineJobTimeData(int jobNo) {
       
        // �Z�[�u�f�[�^�����邩�m�F
        if (PlayerPrefsJsonUtility.ExistsData(WORKING_JOB_SAVE_KEY + jobNo.ToString())) {            // PlayerPrefs.HasKey(WORKING_JOB_SAVE_KEY + jobNo.ToString())
            // �Z�[�u�f�[�^������ꍇ
            JobTimeData jobTimeData = PlayerPrefsJsonUtility.LoadGetObjectData<JobTimeData>(WORKING_JOB_SAVE_KEY + jobNo.ToString());
            AddWorkingJobTimeDatasList(jobTimeData);
            //string json = PlayerPrefs.GetString(WORKING_JOB_SAVE_KEY + jobNo.ToString());
            //workingJobTimeDatasList[jobNo] = JsonUtility.FromJson<JobTimeData>(json);

            DateTime time = jobTimeData.GetDateTime();
            string str =  time.ToString("yyyy/MM/dd HH:mm:ss");
            Debug.Log($"�d���J�n�� : �Z�[�u����Ă������� : {str}");
            Debug.Log($"���[�h���̎c�莞�� : {jobTimeData.elespedJobTime}");
        } 
    }

    public void SetGameManager(GameManager gameManager) {
        this.gameManager = gameManager;
    }

    /// <summary>
    /// ���݂��g������ JobTimeData �̒ǉ�
    /// </summary>
    /// <param name="tapPointDetail"></param>
    public void CreateWorkingJobTimeDatasList(TapPointDetail tapPointDetail) {
        // JobTimeData ��������
        JobTimeData jobTimeData = new JobTimeData { jobNo = tapPointDetail.jobData.jobNo, elespedJobTime = tapPointDetail.jobData.jobTime };
        AddWorkingJobTimeDatasList(jobTimeData);
    }

    /// <summary>
    /// JobTimeData ��ǉ��B���̃��X�g�ɂ����񂪌��݂��g�������Ă�����e�ɂȂ�
    /// </summary>
    /// <param name="jobTimeData"></param>
    public void AddWorkingJobTimeDatasList(JobTimeData jobTimeData) {
        // ���łɃ��X�g�ɂ��邩�m�F
        if (!workingJobTimeDatasList.Exists(x => x.jobNo == jobTimeData.jobNo)) {
            workingJobTimeDatasList.Add(jobTimeData);
            Debug.Log(jobTimeData.elespedJobTime);
        }       
    }

    /// <summary>
    /// ���g���̏I������ JobTimeData ���폜���A�Z�[�u�f�[�^���폜
    /// </summary>
    public void RemoveWorkingJobTimeDatasList(int removeJobNo) {
        // ���X�g����폜
        workingJobTimeDatasList.Remove(workingJobTimeDatasList.Find(x => x.jobNo == removeJobNo));

        // �Z�[�u�f�[�^���폜
        PlayerPrefsJsonUtility.RemoveObjectData(WORKING_JOB_SAVE_KEY + removeJobNo);
    }
}
