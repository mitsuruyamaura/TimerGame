using System.Collections.Generic;

/// <summary>
/// �l�����Ă���J�܂̊Ǘ��p�N���X
/// </summary>
[System.Serializable]
public class Reward
{
    public int rewardPoint;
    public List<RewardInfo> rewardInfosList = new List<RewardInfo>();

    /// <summary>
    /// �V�K�̖J�܃f�[�^�̍쐬�Ə�����
    /// </summary>
    public static Reward Create() {
        Reward reward = new Reward { 
            rewardPoint = 0 
        };

        return reward;
    }
}