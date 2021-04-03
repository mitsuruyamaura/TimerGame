using UnityEngine;

/// <summary>
/// 褒賞のデータ
/// </summary>
[System.Serializable]
public class RewardData
{
    public int rewardNo;
    public RarityType rarityType;   // 希少度
    public int rarityRate;          // 度合
    public Sprite spriteReward;     // 画像
    public int rewardPoint;         // 獲得できるポイント 
}
