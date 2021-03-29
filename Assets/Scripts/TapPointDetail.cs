using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TapPointDetail : MonoBehaviour
{
    [SerializeField]
    private Button btnTapPoint;

    [SerializeField]
    private Image imgTapPoint;

    [SerializeField]
    private Sprite jobSprite;

    [SerializeField]
    private Sprite defaultSprite;

    [SerializeField, Header("���̃^�b�v�|�C���g�̃W���u�ԍ�")]
    private int myJobNo;

    public JobData jobData;

    private GameManager gameManager;

    public bool isJobs;

    Tween tween;

    void Start() {
        btnTapPoint.onClick.AddListener(OnClickTapPoint);
    }

    /// <summary>
    /// TapPointDetail �̐ݒ�@�@=>�@TODO UniRX �ɂ�����
    /// </summary>
    /// <param name="gameManager"></param>
    public void SetUpTapPointDetail(GameManager gameManager) {
        this.gameManager = gameManager;
    }

    /// <summary>
    /// �^�b�v�|�C���g���^�b�v�����ۂ̏���
    /// </summary>
    private void OnClickTapPoint() {
        Debug.Log("TapPoint �^�b�v");

        // �^�b�v�A�j��
        transform.DOPunchScale(Vector3.one * 1.25f, 0.15f).SetEase(Ease.OutBounce);

        // TODO �|�b�v�A�b�v�\�� ���̃N���X�̏���n��
        gameManager.GenerateJobsConfirmPopUp(this);
    }

    /// <summary>
    /// �^�b�v�|�C���g�̕\��/��\���؂�ւ�
    /// </summary>
    /// <param name="isSwitch"></param>
    public void SwtichActivateTapPoint(bool isSwitch) {
        imgTapPoint.enabled = isSwitch;
        btnTapPoint.enabled = isSwitch;

        transform.localScale = Vector3.one;
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
    /// 
    /// </summary>
    /// <returns></returns>
    public IEnumerator StartJobs() {

        while (isJobs) {
            // TODO �����Ƃ��Ď��Ԃ��m�F����

            yield return new WaitForSeconds(3.0f);

            isJobs = false;

            tween.Kill();

            yield return null;
        }

        // �d���I��
        Debug.Log("���g�� �I��");

        // TODO �Q�[���}�l�[�W���[�ŃL��������
        gameManager.GenerateChara(this);

    }
}
