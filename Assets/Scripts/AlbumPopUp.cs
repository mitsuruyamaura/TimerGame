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
    /// AlbumPopUp �̐ݒ�ƕ\��
    /// </summary>
    public void SetUpAlbumPopUp(GameManager gameManager, Vector3 centerPos, Vector3 btnPos) {
        transform.position = btnPos;
        closePos = transform.position;

        btnClose.onClick.AddListener(OnClickCloseAlbum);

        for (int i = 0; i < GameData.instance.GetEarnedRewardsListCount(); i++) {

            // �l�����Ă���J�܂𐶐�
            RewardDetail rewardDetail = Instantiate(rewardDetailPrefab, rewardDetailTran, false);

            // RewardNo ���� RewardData ���擾���� RewardDetail ��ݒ�
            rewardDetail.SetUpRewardDetail(gameManager.GetRewardDataFromRewardNo(i), this);

            // �ŏ��� RewardDetail �������摜�ɐݒ�
            if (rewardDetailsList.Count == 0) {
                imgReward.sprite = gameManager.GetRewardDataFromRewardNo(i).spriteReward;
            }

            // �J�܈ꗗ�� List �ɓo�^
            rewardDetailsList.Add(rewardDetail);
        }

        transform.localScale = Vector3.zero;

        Sequence sequence = DOTween.Sequence();

        sequence.Append(transform.DOLocalMove(centerPos, 0.3f).SetEase(Ease.Linear));

        sequence.Join(transform.DOScale(Vector2.one * 1.2f, 0.5f).SetEase(Ease.InBack)).OnComplete(() => { transform.DOScale(Vector2.one, 0.2f); });// .SetEase(Ease.InBack)).
        //sequence.Join(transform.DOLocalMove(centerPos, 0.3f).SetEase(Ease.Linear));
        // �|�b�v�A�b�v�����X�ɕ\��
        //transform.DOScale(Vector2.one * 1.2f, 0.3f).SetEase(Ease.InBack).OnComplete(() => { transform.localScale = Vector2.one; });
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
        Sequence sequence = DOTween.Sequence();

        sequence.Append(transform.DOScale(Vector2.zero, 0.3f).SetEase(Ease.Linear));
        sequence.Join(transform.DOMove(closePos, 0.3f).SetEase(Ease.Linear)).OnComplete(() => { Destroy(gameObject); });

        //transform.DOScale(Vector2.zero, 0.3f).SetEase(Ease.Linear).OnComplete(() => { Destroy(gameObject); });       
    }
}
