using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class AlbumPopUp : MonoBehaviour
{
    [SerializeField]
    private Image imgReward;

    [SerializeField]
    private RewardDetail rewardDetailPrefab;

    [SerializeField]
    private Transform rewardDetailTran;

    [SerializeField]
    private List<RewardDetail> rewardDetailsList = new List<RewardDetail>();

    [SerializeField]
    private Button btnClose;

    /// <summary>
    /// AlbumPopUp の設定と表示
    /// </summary>
    public void SetUpAlbumPopUp(int rewardCount, GameManager gameManager) {
        btnClose.onClick.AddListener(OnClickCloseAlbum);

        for (int i = 0; i < rewardCount; i++) {
            // 獲得している褒賞か確認
            if (GameData.instance.earnedRewardsList.Exists(x => x.rewardNo == i)) {
                // 獲得している場合には RewardDetail を生成
                RewardDetail rewardDetail = Instantiate(rewardDetailPrefab, rewardDetailTran, false);

                // RewardNo から RewardData を取得して RewardDetail を設定
                rewardDetail.SetUpRewardDetail(gameManager.GetRewardDataFromRewardNo(i), this);

                // 最初の RewardDetail を初期画像に設定
                if (rewardDetailsList.Count == 0) {
                    imgReward.sprite = gameManager.GetRewardDataFromRewardNo(i).spriteReward;
                }

                // List に登録
                rewardDetailsList.Add(rewardDetail);
            }
        }

        // ポップアップを徐々に表示
        transform.DOScale(Vector2.one * 1.2f, 0.3f).SetEase(Ease.InBack).OnComplete(() => { transform.localScale = Vector2.one; });
    }

    /// <summary>
    /// 選択された褒賞を表示
    /// </summary>
    public void DisplayReward(Sprite spriteReward) {
        imgReward.sprite = spriteReward;
    }

    /// <summary>
    /// アルバムポップアップを閉じる
    /// </summary>
    private void OnClickCloseAlbum() {
        // ポップアップを非表示して破壊
        transform.DOScale(Vector2.zero, 0.3f).SetEase(Ease.Linear).OnComplete(() => { Destroy(gameObject); });       
    }
}
