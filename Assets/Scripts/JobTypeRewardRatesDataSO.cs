using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// お使いの種類(難易度)による褒賞の提供割合データベース
/// </summary>
[CreateAssetMenu(fileName = "JobTypeRewardRatesDataSO", menuName = "Create JobTypeRewardRatesDataSO")]
public class JobTypeRewardRatesDataSO : ScriptableObject {

    public List<JobTypeRewardRatesData> jobTypeRewardRatesDataList = new List<JobTypeRewardRatesData>();
}
