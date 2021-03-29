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

    [SerializeField]
    private CharaController charaPrefab;

    [SerializeField]
    private List<CharaController> charasList = new List<CharaController>();

    private JobsConfirmPopUp jobsConfirmPopUp;

    private TapPointDetail selectedTapPointDetail;




    void Awake() {
        // TODO ロード

    }

    void Start() {   // TODO コルーチンにする

        // TODO キャラの生成確認

        TapPointSetUp();   
    }

    private void TapPointSetUp() {
        for (int i = 0; i < tapPointDetailsList.Count; i++) {
            tapPointDetailsList[i].SetUpTapPointDetail(this);

            // TODO ロードしたデータを照合して、お使い中の場合には非表示にする
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
            // ボタンの画像を変更
            selectedTapPointDetail.ChangeJobSprite();

            // 仕事中の状態にする
            selectedTapPointDetail.isJobs = true;

            // 仕事開始
            StartCoroutine(selectedTapPointDetail.StartJobs());
        } else {
            Debug.Log("お使いには行かない");
        }
    }

    /// <summary>
    /// キャラ生成
    /// </summary>
    public void GenerateChara(TapPointDetail tapPointDetail) {
        selectedTapPointDetail = tapPointDetail;
        selectedTapPointDetail.SwtichActivateTapPoint(false);

        // TODO お使い用のキャラの生成
        Debug.Log("お使い用のキャラの生成");
        CharaController chara = Instantiate(charaPrefab, selectedTapPointDetail.transform, false);

        chara.SetUpChara(this, selectedTapPointDetail);
    }

    /// <summary>
    /// 仕事中の状態をセーブ
    /// </summary>
    private void SaveJobs() {
        // 仕事の番号　仕事を始めた時間を記録
    }

    /// <summary>
    /// 仕事中の状態をロード
    /// </summary>
    private void LoadJobs() {

    }
}
