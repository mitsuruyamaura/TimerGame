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
    /// AlbumPopUp �̐ݒ�ƕ\��
    /// </summary>
    public void SetUpAlbumPopUp(int rewardCount, GameManager gameManager) {
        btnClose.onClick.AddListener(OnClickCloseAlbum);

        for (int i = 0; i < rewardCount; i++) {
            // �l�����Ă���J�܂��m�F
            if (GameData.instance.earnedRewardsList.Exists(x => x.rewardNo == i)) {
                // �l�����Ă���ꍇ�ɂ� RewardDetail �𐶐�
                RewardDetail rewardDetail = Instantiate(rewardDetailPrefab, rewardDetailTran, false);

                // RewardNo ���� RewardData ���擾���� RewardDetail ��ݒ�
                rewardDetail.SetUpRewardDetail(gameManager.GetRewardDataFromRewardNo(i), this);

                // �ŏ��� RewardDetail �������摜�ɐݒ�
                if (rewardDetailsList.Count == 0) {
                    imgReward.sprite = gameManager.GetRewardDataFromRewardNo(i).spriteReward;
                }

                // List �ɓo�^
                rewardDetailsList.Add(rewardDetail);
            }
        }

        // �|�b�v�A�b�v�����X�ɕ\��
        transform.DOScale(Vector2.one * 1.2f, 0.3f).SetEase(Ease.InBack).OnComplete(() => { transform.localScale = Vector2.one; });
    }

    /// <summary>
    /// �I�����ꂽ�J�܂�\��
    /// </summary>
    public void DisplayReward(Sprite spriteReward) {
        imgReward.sprite = spriteReward;
    }

    /// <summary>
    /// �A���o���|�b�v�A�b�v�����
    /// </summary>
    private void OnClickCloseAlbum() {
        // �|�b�v�A�b�v���\�����Ĕj��
        transform.DOScale(Vector2.zero, 0.3f).SetEase(Ease.Linear).OnComplete(() => { Destroy(gameObject); });       
    }
}
