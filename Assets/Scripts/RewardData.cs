using UnityEngine;

/// <summary>
/// �J�܂̃f�[�^
/// </summary>
[System.Serializable]
public class RewardData {
    public string rewardName;
    public int rewardNo;
    public RarityType rarityType;   // �󏭓x
    public int rarityRate;          // �x��
    public int rewardSpriteNo;      // �摜�ԍ�
    public int rewardPoint;         // �l���ł���|�C���g 
    public Sprite spriteReward;     // �摜
}