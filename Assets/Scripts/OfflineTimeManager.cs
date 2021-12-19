using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Cysharp.Threading.Tasks;

public class OfflineTimeManager : MonoBehaviour
{
    public static OfflineTimeManager instance;

    private DateTime loadDateTime = new DateTime();   // �O��Q�[�����~�߂����ɃZ�[�u���Ă��鎞��
    public DateTime LoadDateTime { get; set; }

    private int elaspedTime;    // �o�ߎ���

    private GameManager gameManager;

    private const string SAVE_KEY_DATETIME = "OfflineDateTime";
    private const string FORMAT = "yyyy/MM/dd HH:mm:ss";

    //private const string SAVE_KEY_STRING = "OfflineTime";
    private const string WORKING_JOB_SAVE_KEY = "workingJobNo_";

    /// <summary>
    /// ���g���p�̎��ԃf�[�^���Ǘ����邽�߂̃N���X
    /// </summary>
    [Serializable]
    public class JobTimeData {
        public int jobNo;              // ���g���̒ʂ��ԍ�
        public int elaspedJobTime;     // ���g���̎c�莞��
        public string jobTimeString;   // DateTime �N���X�𕶎���ɂ��邽�߂̕ϐ�

        /// <summary>
        /// DateTime �𕶎���^�ŕۑ����Ă���̂ŁADateTime �^�ɖ߂��Ď擾
        /// </summary>
        /// <returns></returns>
        public DateTime GetDateTime() {
            return DateTime.ParseExact(jobTimeString, FORMAT, null);
            //return System.DateTime.FromBinary(System.Convert.ToInt64(jobTimeString));
        }
    }

    [Header("���g���̎��ԃf�[�^�̃��X�g")]
    public List<JobTimeData> workingJobTimeDatasList = new List<JobTimeData>();

    /// <summary>
    /// ���Ԃ��Z�[�u���邽�߂̍\����(�\���̂͒l�^�Ȃ̂ŁA���̃N���X�ŗ��p���Ȃ��ꍇ�ɂ̓N���X�����y���֗�)
    /// </summary>
    [Serializable]
    public struct OfflineTimeData {  // �s�v
        public string dateTimeString;   // DateTime �^�̓V���A���C�Y�ł��Ȃ��̂ŕ�����^�ɂ��邽�߂ɗ��p

        /// <summary>
        /// DateTime �𕶎���^�ŕۑ����Ă���̂ŁADateTime �^�ɖ߂��Ď擾
        /// </summary>
        /// <returns></returns>
        public DateTime GetDateTime() {
            return System.DateTime.FromBinary(System.Convert.ToInt64(dateTimeString));
        }
    }

    [HideInInspector]
    public OfflineTimeData offlineTimeData;  // �s�v


    void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }

        // �Z�[�u�f�[�^�̃��[�h
        //LoadOfflineDateTime();

        // �I�t���C���ł̌o�ߎ��Ԃ��v�Z
        //CalculateOfflineDateTimeElasped(loadDateTime);

        // TODO ���g���̃f�[�^�̃��[�h
        //LoadOfflineJobTimeData(0);
    }

    /// <summary>
    /// �I�t���C���ł̌o�ߎ��Ԃ��v�Z
    /// </summary>
    public int CalculateOfflineDateTimeElasped(DateTime oldDateTime) {
        // ���݂̎��Ԃ��擾
        DateTime currentDateTime = DateTime.Now;

        // �f�[�^�̕s�����o�ߎ��ԂƋ����ԂƂŃ`�F�b�N
        if (oldDateTime > currentDateTime) {
            // �Z�[�u�f�[�^�̎��Ԃ̕������̎��Ԃ����i��ł���ꍇ�ɂ́A���̎��Ԃ����Ȃ���
            oldDateTime = DateTime.Now;
        }

        // �o�߂������Ԃ̍���
        TimeSpan dateTimeElasped = currentDateTime - oldDateTime;

        // https://www.delftstack.com/ja/howto/csharp/covert-decimal-to-int-in-csharp/ cast
        // https://docs.microsoft.com/ja-jp/dotnet/api/system.midpointrounding?view=net-5.0 ToEven
        // https://docs.microsoft.com/ja-jp/dotnet/api/system.timespan.totalseconds?view=net-5.0 TotalSeconds �v���p�e�B
        // Math.Round ���\�b�h�� �����_�̒l���ł��߂������l��10�i�l�Ɋۂ߂邽�߂̃��\�b�h
        // �o�ߎ���(Math.Round ���\�b�h�𗘗p���āAdouble �^�� int �^�ɕϊ��B�����_�� 0 �̈ʂŁA���l�̊ۂ߂̏����̎w��� ToEven(���l�� 2 �̐��l�̒��ԂɈʒu����Ƃ��ɁA�ł��߂������̒l) ���w��) 
        elaspedTime = (int)Math.Round(dateTimeElasped.TotalSeconds, 0, MidpointRounding.ToEven);

        Debug.Log($"�I�t���C���ł̌o�ߎ��� : {elaspedTime} �b");

        DebugManager.instance.DisplayDebugDialog($"�I�t���C���ł̌o�ߎ��� : {elaspedTime} �b");

        return elaspedTime;
    }

    /// <summary>
    /// �Q�[�����I�������Ƃ��Ɏ����I�ɌĂ΂��
    /// </summary>
    private async UniTask OnApplicationQuit() {�@�@�@�@�@�@//�@async UniTask �ɕύX

        // PlayFab �ɃQ�[���I�����̎��Ԃ�ۑ�
        await OnlineTimeManager.UpdateLogOffTimeAsync();

        // PlayFab �Ɏd���̎c�莞�Ԃ�ۑ�
        if (workingJobTimeDatasList.Count > 0) {
            await OnlineTimeManager.UpdateJobTimeAsync(workingJobTimeDatasList);
        }


        SaveOfflineDateTime();
        Debug.Log("�Q�[�����f�B���Ԃ̃Z�[�u����");

        DebugManager.instance.DisplayDebugDialog("�Q�[�����f�B���Ԃ̃Z�[�u����");

        

        // ���g�����̃f�[�^������ꍇ�A���g���̎��ԃf�[�^���Z�[�u
        for (int i = 0; i < workingJobTimeDatasList.Count; i++) {
            SaveWorkingJobTimeData(workingJobTimeDatasList[i].jobNo);
        }
    }

    /// <summary>
    /// �I�t���C���ł̎��Ԃ����[�h
    /// </summary>
    public void LoadOfflineDateTime() {

        // �Z�[�u�f�[�^�����邩�m�F
        if (PlayerPrefsHelper.ExistsData(SAVE_KEY_DATETIME)) {

            string oldDateTimeString = PlayerPrefsHelper.LoadStringData(SAVE_KEY_DATETIME);
            //Debug.Log(oldDateTimeString);

            //if (!string.IsNullOrEmpty(oldDateTimeString))
            loadDateTime = DateTime.ParseExact(oldDateTimeString, FORMAT, null);

            // �Z�[�u�f�[�^������ꍇ
            //offlineTimeData = PlayerPrefsHelper.LoadGetObjectData<OfflineTimeData>(SAVE_KEY_STRING);

            //string json = PlayerPrefs.GetString(SAVE_KEY_STRING);
            //offlineTimeData = JsonUtility.FromJson<OfflineTimeData>(json);

            //oldDateTime = offlineTimeData.GetDateTime();
            //string str = oldDateTime.ToString(FORMAT);
            Debug.Log($"�Q�[���J�n�� : �Z�[�u����Ă������� : {oldDateTimeString}");

            //string str = DateTime.Now.ToString(FORMAT);
            Debug.Log($"���̎��� : {DateTime.Now.ToString(FORMAT)}");

            DebugManager.instance.DisplayDebugDialog($"�Q�[���J�n�� : �Z�[�u����Ă������� : {oldDateTimeString}");
            DebugManager.instance.DisplayDebugDialog($"���̎��� : {DateTime.Now.ToString(FORMAT)}");

        } else {
            // �Z�[�u�f�[�^���Ȃ��ꍇ�A�Z�[�u�f�[�^�p�̍\���̂��쐬
            //offlineTimeData = new OfflineTimeData { dateTimeString = DateTime.Now.ToBinary().ToString() };
            loadDateTime = DateTime.Now;
            string str = loadDateTime.ToString(FORMAT);
            Debug.Log($"�Z�[�u�f�[�^���Ȃ��̂ō��̎��Ԃ��擾 : {str}");

            DebugManager.instance.DisplayDebugDialog($"�Z�[�u�f�[�^���Ȃ��̂ō��̎��Ԃ��擾 : {str}");
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
    public void SaveOfflineDateTime() {
        //offlineTimeData.dateTimeString = DateTime.Now.ToBinary().ToString();

        //string dateTimeString = DateTime.Now.ToBinary().ToString();


        //PlayerPrefsHelper.SaveSetObjectData(offlineTimeData, SAVE_KEY_STRING);


        string dateTimeString = DateTime.Now.ToString(FORMAT);
        PlayerPrefsHelper.SaveStringData(SAVE_KEY_DATETIME, dateTimeString);

        //PlayerPrefs.SetString(SAVE_KEY_DATETIME, dateTimeString);
        //PlayerPrefs.Save();

        //Debug.Log(dateTimeString);

        //string json = JsonUtility.ToJson(offlineTimeData);
        //PlayerPrefs.SetString(SAVE_KEY_STRING, json);
        //PlayerPrefs.Save();

        //string str = DateTime.Now.ToString(FORMAT);
        Debug.Log($"�Q�[���I���� : �Z�[�u���� : {dateTimeString}");

        // �Q�[���I�����Ă���̂ŁA��ʂŌ���Ȃ����ߕs�v
        //DebugManager.instance.DisplayDebugDialog($"�Q�[���I���� : �Z�[�u���� : {dateTimeString}");
    }

    /// <summary>
    ///  �e���g���̎c�莞�Ԃ̍X�V
    /// </summary>
    /// <param name="jobNo"></param>
    /// <param name="currentJobTime"></param>
    public void UpdateCurrentJobTime(int jobNo, int currentJobTime) {
        workingJobTimeDatasList.Find(x => x.jobNo == jobNo).elaspedJobTime = currentJobTime;
    }

    /// <summary>
    /// GameManager �̏����擾
    /// </summary>
    /// <param name="gameManager"></param>
    //public void SetGameManager(GameManager gameManager) {
    //    this.gameManager = gameManager;
    //}

    /// <summary>
    /// List �� JobTimeData ��ǉ��B���̃��X�g�ɂ����񂪌��݂��g�������Ă�����e�ɂȂ�
    /// </summary>
    /// <param name="jobTimeData"></param>
    public void AddWorkingJobTimeDatasList(JobTimeData jobTimeData) {
        // ���łɃ��X�g�ɂ��邩�m�F
        if (!workingJobTimeDatasList.Exists(x => x.jobNo == jobTimeData.jobNo)) {
            workingJobTimeDatasList.Add(jobTimeData);
            Debug.Log(jobTimeData.elaspedJobTime);
        }
    }

    /// <summary>
    /// ���݂��g������ JobTimeData �̍쐬�� List �ւ̒ǉ�
    /// </summary>
    /// <param name="tapPointDetail"></param>
    public void CreateWorkingJobTimeDatasList(TapPointDetail tapPointDetail) {
        // JobTimeData ��������
        JobTimeData jobTimeData = new JobTimeData { jobNo = tapPointDetail.jobData.jobNo, elaspedJobTime = tapPointDetail.jobData.jobTime };
        AddWorkingJobTimeDatasList(jobTimeData);
    }

    /// <summary>
    /// ���g���̎��Ԃ̃Z�[�u
    /// ���g���J�n���ƃQ�[���I�����ɃZ�[�u
    /// </summary>
    /// <param name="jobNo"></param>
    public void SaveWorkingJobTimeData(int jobNo) {

        // �Z�[�u�Ώۂ� JobTimeData ��ݒ�
        JobTimeData jobTimeData = workingJobTimeDatasList.Find(x => x.jobNo == jobNo);

        // ���̎��Ԃ��擾
        //jobTimeData.jobTimeString = DateTime.Now.ToBinary().ToString();

        jobTimeData.jobTimeString = DateTime.Now.ToString(FORMAT);

        // ���݂̂��g���̎c�莞�Ԃ��擾
        //jobTimeData.elaspedJobTime = gameManager.GetTapPointDetailCurrentJobTime(jobNo);
        //Debug.Log(jobTimeData.elespedJobTime);

        PlayerPrefsHelper.SaveSetObjectData(WORKING_JOB_SAVE_KEY + jobTimeData.jobNo.ToString(), jobTimeData);

        //string json = JsonUtility.ToJson(workingJobTimeDatasList[jobNo]);
        //PlayerPrefs.SetString(WORKING_JOB_SAVE_KEY + jobNo.ToString(), json);
        //PlayerPrefs.Save();

        string str = DateTime.Now.ToString(FORMAT);
        Debug.Log($"�d���� : �Z�[�u���� : {str}");
        Debug.Log($"�Z�[�u���̂��g���̎c�莞�� : {jobTimeData.elaspedJobTime}");

        DebugManager.instance.DisplayDebugDialog($"�d���� : �Z�[�u���� : {str}");
        DebugManager.instance.DisplayDebugDialog($"�Z�[�u���̎c�莞�� : {jobTimeData.elaspedJobTime}");
    }

    /// <summary>
    /// �s����̐������A���̍s����� JobTimeData �����邩�ǂ����m�F���A����ꍇ�ɂ̓��[�h���� WorkingJobTimeDatasList �ɒǉ�
    /// </summary>
    public void GetWorkingJobTimeDatasList(List<TapPointDetail> tapPointDetailsList) {
        for (int i = 0; i < tapPointDetailsList.Count; i++) {
            // �Y�����邨�g���̔ԍ��ŃZ�[�u����Ă��鎞�ԃf�[�^�����邩�ǂ����m�F
            LoadOfflineJobTimeData(tapPointDetailsList[i].jobData.jobNo);
        }

        Debug.Log("���g���̃f�[�^���擾");
    }

    /// <summary>
    /// ���g���̊J�n���Ԃ̃��[�h
    /// </summary>
    /// <param name="jobNo"></param>
    public void LoadOfflineJobTimeData(int jobNo) {

        // �Z�[�u�f�[�^�����邩�m�F
        if (PlayerPrefsHelper.ExistsData(WORKING_JOB_SAVE_KEY + jobNo.ToString())) {            // PlayerPrefs.HasKey(WORKING_JOB_SAVE_KEY + jobNo.ToString())
            // �Z�[�u�f�[�^������ꍇ
            JobTimeData jobTimeData = PlayerPrefsHelper.LoadGetObjectData<JobTimeData>(WORKING_JOB_SAVE_KEY + jobNo.ToString());
            AddWorkingJobTimeDatasList(jobTimeData);
            //string json = PlayerPrefs.GetString(WORKING_JOB_SAVE_KEY + jobNo.ToString());
            //workingJobTimeDatasList[jobNo] = JsonUtility.FromJson<JobTimeData>(json);

            DateTime time = jobTimeData.GetDateTime();

            string str = time.ToString(FORMAT);
            Debug.Log($"�d���J�n�� : �Z�[�u����Ă������� : {str}");
            Debug.Log($"���[�h���̎c�莞�� : {jobTimeData.elaspedJobTime}");

            DebugManager.instance.DisplayDebugDialog($"�d���J�n�� : �Z�[�u����Ă������� : {str}");
            DebugManager.instance.DisplayDebugDialog($"���[�h���̎c�莞�� : {jobTimeData.elaspedJobTime}");
        }
    }

    /// <summary>
    /// ���g���̏I������ JobTimeData ���폜���A�Z�[�u�f�[�^���폜
    /// </summary>
    public async void RemoveWorkingJobTimeDatasList(int removeJobNo) {
        // ���X�g����폜
        workingJobTimeDatasList.Remove(workingJobTimeDatasList.Find(x => x.jobNo == removeJobNo));

        // �Z�[�u�f�[�^���폜
        PlayerPrefsHelper.RemoveObjectData(WORKING_JOB_SAVE_KEY + removeJobNo);

        // PlayFab �̂��g���̃f�[�^���X�V
        await OnlineTimeManager.UpdateJobTimeAsync(workingJobTimeDatasList);
    }

    /// <summary>
    /// �f�o�b�O�p
    /// ���ׂĂ̂��g���� JobTimeData ���폜
    /// </summary>
    public void AllRemoveWorkingJobTimeDatasList() {
        // ���X�g���炷�ׂč폜
        workingJobTimeDatasList.Clear();

        // ���ׂẴZ�[�u�f�[�^���폜
        PlayerPrefsHelper.AllClearSaveData();
        DebugManager.instance.DisplayDebugDialog("���ׂẴZ�[�u�f�[�^���폜 ���s");
    }
}
