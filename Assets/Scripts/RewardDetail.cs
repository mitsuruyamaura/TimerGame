using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class RewardDetail : MonoBehaviour
{
    [SerializeField]
    private Image imgReward;

    [SerializeField]
    private RewardData rewardData;

    [SerializeField]
    private Button btnRewardDetail;

    private AlbumPopUp albumPopUp;

    /// <summary>
    /// RewardDetail の設定
    /// </summary>
    /// <param name="rewardData"></param>
    /// <param name="albumPopUp"></param>
    public void SetUpRewardDetail(RewardData rewardData, AlbumPopUp albumPopUp) {
        this.rewardData = rewardData;
        this.albumPopUp = albumPopUp;

        imgReward.sprite = this.rewardData.spriteReward;

        btnRewardDetail.onClick.AddListener(OnClickRewardDetail);
    }

    /// <summary>
    /// RewardDetail をおした際の処理
    /// </summary>
    public void OnClickRewardDetail() {
        albumPopUp.DisplayReward(rewardData.spriteReward);
    }
}
