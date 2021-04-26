using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// お使いのデータ
/// </summary>
[System.Serializable]
public class JobData {
    [Tooltip("お使いの通し番号")]
    public int jobNo;
    [Tooltip("お使いにかかる時間")]
    public int jobTime;
    [Tooltip("お使いの名前")]
    public string jobTitle;
    [Tooltip("お使いの種類・難しさ")]
    public JobType jobType;
}
