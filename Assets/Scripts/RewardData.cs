using UnityEngine;

/// <summary>
/// 褒賞のデータ
/// </summary>
[System.Serializable]
public class RewardData {
    public string rewardName;
    public int rewardNo;
    public RarityType rarityType;   // 希少度
    public int rarityRate;          // 度合
    public int rewardSpriteNo;      // 画像番号
    public int rewardPoint;         // 獲得できるポイント 
    public Sprite spriteReward;     // 画像
}