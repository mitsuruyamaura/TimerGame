using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private List<TapPointDetail> tapPointDetailsList = new List<TapPointDetail>();

    [SerializeField]
    private Transform canvasTran;

    [SerializeField]
    private JobsConfirmPopUp jobsConfirmPopUpPrefab;

    private JobsConfirmPopUp jobsConfirmPopUp;

    private TapPointDetail selectedTapPointDetail;

    void Start() {
        TapPointSetUp();   
    }

    private void TapPointSetUp() {
        for (int i = 0; i < tapPointDetailsList.Count; i++) {
            tapPointDetailsList[i].SetUpTapPointDetail(this);
        }
    }

    /// <summary>
    /// TapPoint をクリックした際にお使い確認用のポップアップを開く
    /// </summary>
    /// <param name="tapPointDetail"></param>
    public void GenerateJobsConfirmPopUp(TapPointDetail tapPointDetail) {
        selectedTapPointDetail = tapPointDetail;

        // TODO ポップアップをインスタンスする 
        Debug.Log("お使い確認用のポップアップを開く");

        jobsConfirmPopUp = Instantiate(jobsConfirmPopUpPrefab, canvasTran, false);

        // TODO ポップアップに JobData を送る
        jobsConfirmPopUp.OpenPopUp(this, selectedTapPointDetail.jobData);
    }

    /// <summary>
    /// お使いを引き受けたか確認
    /// </summary>
    /// <param name="isSubmit"></param>
    public void JudgeSubmitJob(bool isSubmit) {
        if (isSubmit) {
            // TODO お使い用のキャラの生成
            Debug.Log("お使い用のキャラの生成");

            // ボタンを非表示
            selectedTapPointDetail.SwtichActivateTapPoint(false);
        } else {
            Debug.Log("お使いには行かない");
        }
    }
}
