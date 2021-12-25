using System.Collections.Generic;

/// <summary>
/// 獲得している褒賞の管理用クラス
/// </summary>
[System.Serializable]
public class Reward
{
    public int rewardPoint;
    public List<RewardInfo> rewardInfosList = new List<RewardInfo>();

    /// <summary>
    /// 新規の褒賞データの作成と初期化
    /// </summary>
    public static Reward Create() {
        Reward reward = new Reward { 
            rewardPoint = 0 
        };

        return reward;
    }
}