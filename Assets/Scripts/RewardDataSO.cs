using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �J�܂̃f�[�^�x�[�X
/// </summary>
[CreateAssetMenu(fileName = "RewardDataList", menuName = "Create RewardDataList")]
public class RewardDataSO : ScriptableObject
{
    public List<RewardData> rewardDatasList = new List<RewardData>();
}
