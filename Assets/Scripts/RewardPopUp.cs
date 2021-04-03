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
    /// ポップアップの設定と表示
    /// </summary>
    public void SetUpRewardPopUp(RewardData rewardData) {
        canvasGroup.alpha = 0;

        canvasGroup.DOFade(1.0f, 0.5f).SetEase(Ease.Linear);

        // ボタンにメソッドの登録
        btnSubmit.onClick.AddListener(OnClickCloseRewardPopUp);

        // 褒賞ポイント表示
        txtRewardPoint.text = rewardData.rewardPoint.ToString();

        // 希少度の表示
        txtRarity.text = rewardData.rarityType.ToString();

        // 画僧の設定
        imgReward.sprite = rewardData.spriteReward;

        // TODO 演出

    }

    /// <summary>
    /// ポップアップ非表示
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
