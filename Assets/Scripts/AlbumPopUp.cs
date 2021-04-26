using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class AlbumPopUp : MonoBehaviour
{
    private Vector3 closePos;

    [SerializeField]
    private Button btnClose;

    [SerializeField]
    private Image imgReward;

    [SerializeField]
    private RewardDetail rewardDetailPrefab;

    [SerializeField]
    private Transform rewardDetailTran;

    [SerializeField]
    private List<RewardDetail> rewardDetailsList = new List<RewardDetail>();


    /// <summary>
    /// AlbumPopUp の設定と表示
    /// </summary>
    public void SetUpAlbumPopUp(GameManager gameManager, Vector3 centerPos, Vector3 btnPos) {
        transform.position = btnPos;
        closePos = transform.position;

        btnClose.onClick.AddListener(OnClickCloseAlbum);

        for (int i = 0; i < GameData.instance.GetEarnedRewardsListCount(); i++) {

            // 獲得している褒賞を生成
            RewardDetail rewardDetail = Instantiate(rewardDetailPrefab, rewardDetailTran, false);

            // RewardNo から RewardData を取得して RewardDetail を設定
            rewardDetail.SetUpRewardDetail(gameManager.GetRewardDataFromRewardNo(i), this);

            // 最初の RewardDetail を初期画像に設定
            if (rewardDetailsList.Count == 0) {
                imgReward.sprite = gameManager.GetRewardDataFromRewardNo(i).spriteReward;
            }

            // 褒賞一覧の List に登録
            rewardDetailsList.Add(rewardDetail);
        }

        transform.localScale = Vector3.zero;

        Sequence sequence = DOTween.Sequence();

        sequence.Append(transform.DOLocalMove(centerPos, 0.3f).SetEase(Ease.Linear));

        sequence.Join(transform.DOScale(Vector2.one * 1.2f, 0.5f).SetEase(Ease.InBack)).OnComplete(() => { transform.DOScale(Vector2.one, 0.2f); });// .SetEase(Ease.InBack)).
        //sequence.Join(transform.DOLocalMove(centerPos, 0.3f).SetEase(Ease.Linear));
        // ポップアップを徐々に表示
        //transform.DOScale(Vector2.one * 1.2f, 0.3f).SetEase(Ease.InBack).OnComplete(() => { transform.localScale = Vector2.one; });
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
        Sequence sequence = DOTween.Sequence();

        sequence.Append(transform.DOScale(Vector2.zero, 0.3f).SetEase(Ease.Linear));
        sequence.Join(transform.DOMove(closePos, 0.3f).SetEase(Ease.Linear)).OnComplete(() => { Destroy(gameObject); });

        //transform.DOScale(Vector2.zero, 0.3f).SetEase(Ease.Linear).OnComplete(() => { Destroy(gameObject); });       
    }
}
