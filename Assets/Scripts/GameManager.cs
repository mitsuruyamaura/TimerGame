using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

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
        // TODO お使いのデータのロード
        OfflineTimeManager.instance.LoadOfflineJobTimeData(0);
    }

    void Start() {   // TODO コルーチンにする

        // TODO キャラの生成確認

        TapPointSetUp();   
    }

    private void TapPointSetUp() {
        for (int i = 0; i < tapPointDetailsList.Count; i++) {
            tapPointDetailsList[i].SetUpTapPointDetail(this);

            // TODO ロードしたデータを照合して、お使い中の場合には非表示にする
            if (JudgeJobsEnd(tapPointDetailsList[i])) {
                GenerateChara(tapPointDetailsList[i]);
            }
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

            // 仕事開始時間のセーブ
            OfflineTimeManager.instance.SaveWorkingJobTimeData(selectedTapPointDetail.jobData.jobNo);

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

    private bool JudgeJobsEnd(TapPointDetail tapPointDetail) {

        // 目標となる時間を TimeSpan にする
        //TimeSpan addTime = new TimeSpan(0, 0, tapPointDetail.jobData.jobTime, 0);

        // TODO Offline の List と引数の情報を確認

        // お使い開始の時間に目標値を加算する
        //DateTime resultTime = OfflineTimeManager.instance.workingJobTimeDatasList[0].GetDateTime() + addTime;

        // 現在の時間を取得する
        DateTime currentDateTime = DateTime.Now;

        // 経過した時間の差分
        TimeSpan timeElasped = currentDateTime - OfflineTimeManager.instance.workingJobTimeDatasList[0].GetDateTime();

        // 差分値を float 型に変換
        float elapsedTime = (int)Math.Round(timeElasped.TotalSeconds, 0, MidpointRounding.ToEven);
        Debug.Log("お使い時間の差分 : " + elapsedTime + " : 秒");

        // 経過時間がお使いにかかる時間よりも同じか多いなら
        if ((float)tapPointDetail.jobData.jobTime <= elapsedTime) {
            return true;
        }

        return false;
    }
}
