using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UniRx;

public class TapPointDetail : MonoBehaviour
{
    [SerializeField]
    private Button btnTapPoint;

    [SerializeField, HideInInspector]
    private Transform canvasTran;                 // �|�b�v�A�b�v�̐����ʒu�̏���o�^���邽�߂̕ϐ�

    [SerializeField,HideInInspector]
    private GameObject jobsComfirmPopUpPrefab;    // �N���[������v���t�@�u��o�^���邽�߂̕ϐ�

    [SerializeField, Header("���̍s����(�^�b�v�|�C���g)�̂��g���ԍ�")]
    private int myJobNo;

    public JobData jobData;

    [SerializeField]
    private Image imgTapPoint;

    [SerializeField]
    private Sprite jobSprite;

    [SerializeField]
    private Sprite defaultSprite;

    private Tween tween;

    private int currentJobTime;

    //private bool isJobs;

    ///// <summary>
    ///// isJobs �ϐ��̃v���p�e�B
    ///// </summary>
    //public bool IsJobs
    //{
    //    set {
    //        isJobs = value;
    //    }
    //    get {
    //        return isJobs;
    //    }
    //}

    [SerializeField, HideInInspector]
    private GameObject charaDetailPrefab;


    //private GameManager gameManager;

    public ReactiveProperty<bool> JobReactiveProperty = new ReactiveProperty<bool>();

    public ReactiveProperty<bool> ButtonReactiveProperty = new ReactiveProperty<bool>(false);


    //void Start() {
    //    btnTapPoint.onClick.AddListener(OnClickTapPoint);
    //}

    /// <summary>
    /// TapPointDetail �̐ݒ�@�@=>�@TODO UniRX �ɂ�����
    /// </summary>
    /// <param name="gameManager"></param>
    public void SetUpTapPointDetail(JobData jobData) {     // GameManager gameManager, 
        btnTapPoint.onClick.AddListener(OnClickTapPoint);
        //this.gameManager = gameManager;

        this.jobData = jobData;
    }

    /// <summary>
    /// �^�b�v�|�C���g���^�b�v�����ۂ̏���
    /// </summary>
    private void OnClickTapPoint() {
        Debug.Log("TapPoint �^�b�v");

        // TODO �^�b�v�A�j�����o
        //Debug.Log("TapPoint �^�b�v�A�j�����o");

        // TODO �s���挈��p�̃|�b�v�A�b�v�\��
        //Debug.Log("TapPoint �s���挈��p�̃|�b�v�A�b�v�\��");

        // �^�b�v�A�j��
        transform.DOPunchScale(Vector3.one * 1.25f, 0.15f).SetEase(Ease.OutBounce);

        ButtonReactiveProperty.Value = true;

        // TODO �|�b�v�A�b�v�\�� ���̃N���X�̏���n��
        //gameManager.GenerateJobsConfirmPopUp(this);

        //GameObject jobsComfirmPopUp = Instantiate(jobsComfirmPopUpPrefab, canvasTran, false);
        //jobsComfirmPopUp.GetComponent<JobsConfirmPopUp>().OpenPopUp(this);
    }

    /// <summary>
    /// ���g���̏���
    /// </summary>
    public void PrapareteJobs(int remainingTime) {
        ChangeJobSprite();
        //IsJobs = true;

        JobReactiveProperty.Value = true;

        // ���g���J�n
        StartCoroutine(WorkingJobs(remainingTime));
    }

    /// <summary>
    /// �d�����̉摜�ɕύX
    /// </summary>
    public void ChangeJobSprite() {
        imgTapPoint.sprite = jobSprite;
        transform.localScale = new Vector3(1.75f, 1.0f, 1.0f);

        tween = transform.DOPunchPosition(new Vector3(10.0f, 0, 0), 3.0f, 10, 3)
            .SetEase(Ease.Linear)
            .OnComplete(() => { })            
            .SetLoops(-1, LoopType.Restart);
    }

    /// <summary>
    /// ���g���̏���
    /// </summary>
    /// <param name="normaJobTime"></param>
    /// <returns></returns>
    public IEnumerator WorkingJobs(int normaJobTime) {

        // �c���Ă��邨�g���̎��Ԃ�ݒ�
        currentJobTime = normaJobTime;

        // ���g�����I��邩���Ď�
        while (JobReactiveProperty.Value) {   // IsJob
            // TODO �����Ƃ��Ď��Ԃ��m�F����
            currentJobTime--;

            // �c�莞�Ԃ��X�V
            OfflineTimeManager.instance.UpdateCurrentJobTime(jobData.jobNo, currentJobTime);

            // �c�莞�Ԃ� 0 �ȉ��ɂȂ�����
            if (currentJobTime <= 0) {
                KillTween();
                //IsJobs = false;

                JobReactiveProperty.Value = false;
            }
            //yield return new WaitForSeconds(3.0f);
           
            yield return null;
        }

        // ���g���Ɋւ������������Ԃɖ߂�
        ReturnDefaultState();

        // �d���I��
        Debug.Log("���g�� �I��");

        // TODO �Q�[���}�l�[�W���[�ŃL��������
        //gameManager.GenerateCharaDetail(this);
        //GenerateCharaDetail();
    }

    /// <summary>
    /// Tween ��j��
    /// </summary>
    public void KillTween() {
        tween.Kill();
    }

    /// <summary>
    /// ���g���Ɋւ������������Ԃɖ߂�
    /// </summary>
    public void ReturnDefaultState() {
        // ���g�����̉摜�����̉摜�ɖ߂�
        imgTapPoint.sprite = defaultSprite;

        // ���g���̎��Ԃ����Z�b�g
        currentJobTime = 0;

        // �I�u�W�F�N�g�̃T�C�Y�������l�ɖ߂�
        transform.localScale = Vector3.one;
    }

    /// <summary>
    /// CharaDetail �𐶐�
    /// </summary>
    //private void GenerateCharaDetail() {
    //    Instantiate(charaDetailPrefab, transform, false);
    //}

    // mi

    /// <summary>
    /// MyJobNo ���擾
    /// </summary>
    /// <returns></returns>
    public int GetMyJobNo() {
        return myJobNo;
    }


    /// <summary>
    /// �c���Ă��邨�g���̎��Ԃ��擾(�Ăяo�����̃��\�b�h�𗘗p���Ă��Ȃ����߁A�s�v)
    /// </summary>
    /// <returns></returns>
    public int GetCurrentJobTime() {
        return currentJobTime;
    }

    /// <summary>
    /// �^�b�v�|�C���g�̕\��/��\���؂�ւ�(�p�ӂ��Ă��邪���ނł͗��p���Ă��Ȃ����߁A�s�v)
    /// </summary>
    /// <param name="isSwitch"></param>
    public void SwtichActivateTapPoint(bool isSwitch) {
        imgTapPoint.enabled = isSwitch;
        btnTapPoint.enabled = isSwitch;
    }
}
