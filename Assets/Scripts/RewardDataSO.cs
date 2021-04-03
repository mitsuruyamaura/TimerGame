using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 褒賞のデータベース
/// </summary>
[CreateAssetMenu(fileName = "RewardDataList", menuName = "Create RewardDataList")]
public class RewardDataSO : ScriptableObject
{
    public List<RewardData> rewardDatasList = new List<RewardData>();
}
