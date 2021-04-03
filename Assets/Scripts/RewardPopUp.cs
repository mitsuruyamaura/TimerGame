using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class RewardPopUp : MonoBehaviour
{
    [SerializeField]
    private Button btnSubmit;

    [SerializeField]
    private Image imgReward;

    [SerializeField]
    private Text txtRewardPoint;

    [SerializeField]
    private Text txtRarity;

    [SerializeField]
    private CanvasGroup canvasGroup;


    /// <summary>
    /// �|�b�v�A�b�v�̐ݒ�ƕ\��
    /// </summary>
    public void SetUpRewardPopUp(RewardData rewardData) {
        canvasGroup.alpha = 0;

        canvasGroup.DOFade(1.0f, 0.5f).SetEase(Ease.Linear);

        // �{�^���Ƀ��\�b�h�̓o�^
        btnSubmit.onClick.AddListener(OnClickCloseRewardPopUp);

        // �J�܃|�C���g�\��
        txtRewardPoint.text = rewardData.rewardPoint.ToString();

        // �󏭓x�̕\��
        txtRarity.text = rewardData.rarityType.ToString();

        // ��m�̐ݒ�
        imgReward.sprite = rewardData.spriteReward;

        // TODO ���o

    }

    /// <summary>
    /// �|�b�v�A�b�v��\��
    /// </summary>
    private void OnClickCloseRewardPopUp() {
        canvasGroup.DOFade(0.0f, 0.5f)
            .SetEase(Ease.Linear)
            .OnComplete(() => 
            {
                Destroy(gameObject);
            });
    }
}
