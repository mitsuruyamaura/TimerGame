using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private List<TapPointDetail> tapPointDetailsList = new List<TapPointDetail>();

    void Start() {
        TapPointSetUp();   
    }

    private void TapPointSetUp() {
        for (int i = 0; i < tapPointDetailsList.Count; i++) {
            tapPointDetailsList[i].SetUpTapPointDetail(this);
        }
    }

    /// <summary>
    /// TapPoint ���N���b�N�����ۂɂ��g���m�F�p�̃|�b�v�A�b�v���J��
    /// </summary>
    /// <param name="tapPointDetail"></param>
    public void OpenJobsConfirmPopUp(TapPointDetail tapPointDetail) {

        // TODO �|�b�v�A�b�v���C���X�^���X���� 
        Debug.Log("���g���m�F�p�̃|�b�v�A�b�v���J��");

        // TODO �|�b�v�A�b�v�� JobData �𑗂�


    }
}
