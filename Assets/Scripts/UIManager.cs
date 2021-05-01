using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Text txtPoint;

    void Start()
    {
        // ReactivePropery ����ʒm���󂯎��(�w��)��
        GameData.instance.PointReactiveProperty.Subscribe(x => txtPoint.text = GameData.instance.PointReactiveProperty.Value.ToString());
    }
}
