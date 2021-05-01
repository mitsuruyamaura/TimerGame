using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UniRx;

public class TapPointDetail : MonoBehaviour
{
    [SerializeField]
    private Button btnTapPoint;

    [SerializeField, HideInInspector]
    private Transform canvasTran;                 // ポップアップの生成位置の情報を登録するための変数

    [SerializeField,HideInInspector]
    private GameObject jobsComfirmPopUpPrefab;    // クローンするプレファブを登録するための変数

    [SerializeField, Header("この行き先(タップポイント)のお使い番号")]
    private int myJobNo;

    public JobData jobData;

    [SerializeField]
    private Image imgTapPoint;

    [SerializeField]
    private Sprite jobSprite;

    [SerializeField]
    private Sprite defaultSprite;

    private Tween tween;

    private int currentJobTime;

    //private bool isJobs;

    ///// <summary>
    ///// isJobs 変数のプロパティ
    ///// </summary>
    //public bool IsJobs
    //{
    //    set {
    //        isJobs = value;
    //    }
    //    get {
    //        return isJobs;
    //    }
    //}

    [SerializeField, HideInInspector]
    private GameObject charaDetailPrefab;


    //private GameManager gameManager;

    public ReactiveProperty<bool> JobReactiveProperty = new ReactiveProperty<bool>();

    public ReactiveProperty<bool> ButtonReactiveProperty = new ReactiveProperty<bool>(false);


    //void Start() {
    //    btnTapPoint.onClick.AddListener(OnClickTapPoint);
    //}

    /// <summary>
    /// TapPointDetail の設定　　=>　TODO UniRX にしたい
    /// </summary>
    /// <param name="gameManager"></param>
    public void SetUpTapPointDetail(JobData jobData) {     // GameManager gameManager, 
        btnTapPoint.onClick.AddListener(OnClickTapPoint);
        //this.gameManager = gameManager;

        this.jobData = jobData;
    }

    /// <summary>
    /// タップポイントをタップした際の処理
    /// </summary>
    private void OnClickTapPoint() {
        Debug.Log("TapPoint タップ");

        // TODO タップアニメ演出
        //Debug.Log("TapPoint タップアニメ演出");

        // TODO 行き先決定用のポップアップ表示
        //Debug.Log("TapPoint 行き先決定用のポップアップ表示");

        // タップアニメ
        transform.DOPunchScale(Vector3.one * 1.25f, 0.15f).SetEase(Ease.OutBounce);

        ButtonReactiveProperty.Value = true;

        // TODO ポップアップ表示 このクラスの情報を渡す
        //gameManager.GenerateJobsConfirmPopUp(this);

        //GameObject jobsComfirmPopUp = Instantiate(jobsComfirmPopUpPrefab, canvasTran, false);
        //jobsComfirmPopUp.GetComponent<JobsConfirmPopUp>().OpenPopUp(this);
    }

    /// <summary>
    /// お使いの準備
    /// </summary>
    public void PrapareteJobs(int remainingTime) {
        ChangeJobSprite();
        //IsJobs = true;

        JobReactiveProperty.Value = true;

        // お使い開始
        StartCoroutine(WorkingJobs(remainingTime));
    }

    /// <summary>
    /// 仕事中の画像に変更
    /// </summary>
    public void ChangeJobSprite() {
        imgTapPoint.sprite = jobSprite;
        transform.localScale = new Vector3(1.75f, 1.0f, 1.0f);

        tween = transform.DOPunchPosition(new Vector3(10.0f, 0, 0), 3.0f, 10, 3)
            .SetEase(Ease.Linear)
            .OnComplete(() => { })            
            .SetLoops(-1, LoopType.Restart);
    }

    /// <summary>
    /// お使いの処理
    /// </summary>
    /// <param name="normaJobTime"></param>
    /// <returns></returns>
    public IEnumerator WorkingJobs(int normaJobTime) {

        // 残っているお使いの時間を設定
        currentJobTime = normaJobTime;

        // お使いが終わるかを監視
        while (JobReactiveProperty.Value) {   // IsJob
            // TODO 条件として時間を確認する
            currentJobTime--;

            // 残り時間を更新
            OfflineTimeManager.instance.UpdateCurrentJobTime(jobData.jobNo, currentJobTime);

            // 残り時間が 0 以下になったら
            if (currentJobTime <= 0) {
                KillTween();
                //IsJobs = false;

                JobReactiveProperty.Value = false;
            }
            //yield return new WaitForSeconds(3.0f);
           
            yield return null;
        }

        // お使いに関する情報を初期状態に戻す
        ReturnDefaultState();

        // 仕事終了
        Debug.Log("お使い 終了");

        // TODO ゲームマネージャーでキャラ生成
        //gameManager.GenerateCharaDetail(this);
        //GenerateCharaDetail();
    }

    /// <summary>
    /// Tween を破棄
    /// </summary>
    public void KillTween() {
        tween.Kill();
    }

    /// <summary>
    /// お使いに関する情報を初期状態に戻す
    /// </summary>
    public void ReturnDefaultState() {
        // お使い中の画像を元の画像に戻す
        imgTapPoint.sprite = defaultSprite;

        // お使いの時間をリセット
        currentJobTime = 0;

        // オブジェクトのサイズを初期値に戻す
        transform.localScale = Vector3.one;
    }

    /// <summary>
    /// CharaDetail を生成
    /// </summary>
    //private void GenerateCharaDetail() {
    //    Instantiate(charaDetailPrefab, transform, false);
    //}

    // mi

    /// <summary>
    /// MyJobNo を取得
    /// </summary>
    /// <returns></returns>
    public int GetMyJobNo() {
        return myJobNo;
    }


    /// <summary>
    /// 残っているお使いの時間を取得(呼び出し元のメソッドを利用していないため、不要)
    /// </summary>
    /// <returns></returns>
    public int GetCurrentJobTime() {
        return currentJobTime;
    }

    /// <summary>
    /// タップポイントの表示/非表示切り替え(用意しているが教材では利用していないため、不要)
    /// </summary>
    /// <param name="isSwitch"></param>
    public void SwtichActivateTapPoint(bool isSwitch) {
        imgTapPoint.enabled = isSwitch;
        btnTapPoint.enabled = isSwitch;
    }
}
