using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UniRx;

public class CharaDetail : MonoBehaviour
{
    [SerializeField]
    private Button btnChara;

    [SerializeField, HideInInspector]
    private Image imgChara;

    [SerializeField, HideInInspector]
    private Sprite successSprite;

    [SerializeField, HideInInspector]
    private Sprite failureSprite;

    private GameManager gameManager;

    private TapPointDetail tapPointDetail;

    public ReactiveProperty<bool> ButtonReactiveProperty = new ReactiveProperty<bool>(false);


    //void Start() {
    //    btnChara.interactable = false;

    //    btnChara.onClick.AddListener(OnClickChara);

    //    btnChara.interactable = true;
    //}

    /// <summary>
    /// キャラの設定
    /// </summary>
    /// <param name="gameManager"></param>
    /// <param name="tapPointDetail"></param>
    /// 
    //public void SetUpCharaDetail() {    // GameManager gameManager, TapPointDetail tapPointDetail
    //this.gameManager = gameManager;
    //this.tapPointDetail = tapPointDetail;

    private void Start() {
        btnChara.interactable = false;

        btnChara.onClick.AddListener(OnClickChara);
        btnChara.interactable = true;   
    }

    /// <summary>
    /// キャラをタップした際の処理
    /// </summary>
    private void OnClickChara() {
        ButtonReactiveProperty.Value = true;

        //gameManager.ResultJobs(tapPointDetail);

        Debug.Log("お使いの結果を表示");
        Destroy(gameObject);
    }
    private void OnDestroy() {
        Debug.Log("キャラ 破棄完了");
    }

    // mi

    /// <summary>
    /// 仕事の成否状態でスプライトを変更
    /// </summary>
    public void ChangeSprite(bool isSuccess) {
        if (isSuccess) {
            imgChara.sprite = successSprite;
        } else {
            imgChara.sprite = failureSprite;
        }
    }
}
