using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CharaController : MonoBehaviour
{
    [SerializeField]
    private Button btnChara;

    [SerializeField]
    private Image imgChara;

    [SerializeField]
    private Sprite successSprite;

    [SerializeField]
    private Sprite failureSprite;

    private GameManager gameManager;

    private TapPointDetail tapPointDetail;

    /// <summary>
    /// �L�����̐ݒ�
    /// </summary>
    /// <param name="gameManager"></param>
    /// <param name="tapPointDetail"></param>
    public void SetUpChara(GameManager gameManager, TapPointDetail tapPointDetail) {
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
        Debug.Log("���� ���\");


    }

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
