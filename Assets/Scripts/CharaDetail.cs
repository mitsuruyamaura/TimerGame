using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

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


    //void Start() {
    //    btnChara.interactable = false;

    //    btnChara.onClick.AddListener(OnClickChara);

    //    btnChara.interactable = true;
    //}

    /// <summary>
    /// �L�����̐ݒ�
    /// </summary>
    /// <param name="gameManager"></param>
    /// <param name="tapPointDetail"></param>
    public void SetUpCharaDetail(GameManager gameManager, TapPointDetail tapPointDetail) {
        this.gameManager = gameManager;
        this.tapPointDetail = tapPointDetail;

        btnChara.interactable = false;

        btnChara.onClick.AddListener(OnClickChara);
        btnChara.interactable = true;
    }

    /// <summary>
    /// �L�������^�b�v�����ۂ̏���
    /// </summary>
    private void OnClickChara() {
        gameManager.ResultJobs(tapPointDetail);

        Debug.Log("���g���̌��ʂ�\��");
        Destroy(gameObject);
    }

    // mi

    /// <summary>
    /// �d���̐��ۏ�ԂŃX�v���C�g��ύX
    /// </summary>
    public void ChangeSprite(bool isSuccess) {
        if (isSuccess) {
            imgChara.sprite = successSprite;
        } else {
            imgChara.sprite = failureSprite;
        }
    }
}
