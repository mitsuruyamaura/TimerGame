using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TapPointDetail : MonoBehaviour
{
    [SerializeField]
    private Button btnTapPoint;

    [SerializeField, Header("このタップポイントのジョブ番号")]
    private int myJobNo;

    [SerializeField]
    private JobData jobData;

    private GameManager gameManager;

    void Start() {
        btnTapPoint.onClick.AddListener(OnClickTapPoint);
    }

    /// <summary>
    /// TapPointDetail の設定　　=>　TODO UniRX にしたい
    /// </summary>
    /// <param name="gameManager"></param>
    public void SetUpTapPointDetail(GameManager gameManager) {
        this.gameManager = gameManager;
    }

    /// <summary>
    /// タップポイントをタップした際の処理
    /// </summary>
    private void OnClickTapPoint() {
        Debug.Log("TapPoint タップ");

        // タップアニメ
        transform.DOPunchScale(Vector3.one * 1.25f, 0.15f).SetEase(Ease.OutBounce);

        // TODO ポップアップ表示 このクラスの情報を渡す
        gameManager.OpenJobsConfirmPopUp(this);
    }

    /// <summary>
    /// タップポイントの表示/非表示切り替え
    /// </summary>
    /// <param name="isSwitch"></param>
    public void SwtichActivateTapPoint(bool isSwitch) {
        btnTapPoint.gameObject.SetActive(isSwitch);
    }
}
