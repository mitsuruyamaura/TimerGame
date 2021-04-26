using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class JobsConfirmPopUp : MonoBehaviour
{
    [SerializeField]
    private Button btnSubmit;

    [SerializeField]
    private Button btnCancel;

    [SerializeField]
    private CanvasGroup canvasGroup;

    [SerializeField]
    private Text txtJobTime;

    [SerializeField]
    private Text txtJobType;

    [SerializeField]
    private Text txtJobTitle;

    private TapPointDetail tapPointDetail;


    // mi
    private GameManager gameManager;


    /// <summary>
    /// ポップアップを設定して開く
    /// </summary>
    /// <param name="gameManager"></param>
    public void OpenPopUp(TapPointDetail tapPointDetail, GameManager gameManager) {
        this.gameManager = gameManager;
        this.tapPointDetail = tapPointDetail;

        SwitchButtons(false);
        btnSubmit.onClick.AddListener(OnClickSubmit);
        btnCancel.onClick.AddListener(OnClickCancel);

        canvasGroup.alpha = 0.0f;

        txtJobTime.text = (tapPointDetail.jobData.jobTime / 100).ToString("F0");
        txtJobType.text = tapPointDetail.jobData.jobType.ToString();
        txtJobTitle.text = tapPointDetail.jobData.jobTitle;

        canvasGroup.DOFade(1.0f, 0.3f)
            .SetEase(Ease.Linear)
            .OnComplete(() => {
                SwitchButtons(true);
            });
    }

    //public void OpenPopUp(TapPointDetail tapPointDetail) {
    //    SwitchButtons(false);
    //    btnSubmit.onClick.AddListener(OnClickSubmit);
    //    btnCancel.onClick.AddListener(OnClickCancel);

    //    canvasGroup.alpha = 0.0f;

    //    this.tapPointDetail = tapPointDetail;

    //    txtJobTime.text = (tapPointDetail.jobData.jobTime / 150).ToString("F0");
    //    txtJobType.text = tapPointDetail.jobData.jobType.ToString();
    //    txtJobTitle.text = tapPointDetail.jobData.jobTitle;

    //    canvasGroup.DOFade(1.0f, 0.3f)
    //        .SetEase(Ease.Linear)
    //        .OnComplete(() => {
    //            SwitchButtons(true);
    //        });
    //}

    /// <summary>
    /// 決定
    /// </summary>
    private void OnClickSubmit() {
        ClosePopUp(true);

        Debug.Log("お使いに行く");
    }

    /// <summary>
    /// キャンセル
    /// </summary>
    private void OnClickCancel() {
        ClosePopUp(false);

        Debug.Log("お使いには行かない");
    }

    /// <summary>
    /// ポップアップを閉じる
    /// </summary>
    /// <param name="isSubmit"></param>
    private void ClosePopUp(bool isSubmit) {
        SwitchButtons(false);

        canvasGroup.DOFade(0f, 0.3f)
            .SetEase(Ease.Linear)
            .OnComplete(() => {
                gameManager.JudgeSubmitJob(isSubmit, tapPointDetail);

                //if (isSubmit) {
                //    tapPointDetail.PrapareteJobs();
                //}

                Destroy(gameObject);
            });
    }

    /// <summary>
    /// すべてのボタンの活性化/非活性化の制御
    /// </summary>
    /// <param name="isSwitch"></param>
    private void SwitchButtons(bool isSwitch) {
        btnSubmit.interactable = isSwitch;
        btnCancel.interactable = isSwitch;
    }
}
