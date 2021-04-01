using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TapPointDetail : MonoBehaviour
{
    [SerializeField]
    private Button btnTapPoint;

    [SerializeField]
    private Image imgTapPoint;

    [SerializeField]
    private Sprite jobSprite;

    [SerializeField]
    private Sprite defaultSprite;

    [SerializeField, Header("このタップポイントのジョブ番号")]
    private int myJobNo;

    public JobData jobData;

    private GameManager gameManager;

    public bool isJobs;

    Tween tween;

    int currentJobTime;


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
        gameManager.GenerateJobsConfirmPopUp(this);
    }

    /// <summary>
    /// タップポイントの表示/非表示切り替え
    /// </summary>
    /// <param name="isSwitch"></param>
    public void SwtichActivateTapPoint(bool isSwitch) {
        imgTapPoint.enabled = isSwitch;
        btnTapPoint.enabled = isSwitch;

        transform.localScale = Vector3.one;
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
        while (isJobs) {
            // TODO 条件として時間を確認する
            currentJobTime--;

            // 残り時間が 0 以下になったら
            if (currentJobTime <= 0) {
                KillTween();
                isJobs = false;                
            }
            //yield return new WaitForSeconds(3.0f);
           
            yield return null;
        }

        currentJobTime = 0;

        // 仕事終了
        Debug.Log("お使い 終了");

        // TODO ゲームマネージャーでキャラ生成
        gameManager.GenerateChara(this);
    }

    /// <summary>
    /// 残っているお使いの時間を取得
    /// </summary>
    /// <returns></returns>
    public int GetCurrentJobTime() {
        return currentJobTime;
    }

    /// <summary>
    /// Tween を破棄
    /// </summary>
    public void KillTween() {
        tween.Kill();
    }

    public void ChangeDefaultSprite() {
        imgTapPoint.sprite = defaultSprite;
    }
}
