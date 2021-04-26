/// <summary>
/// お使いの種類(難易度)による褒賞の提供割合データ
/// </summary>
[System.Serializable]
public class JobTypeRewardRatesData {
    public JobType jobType;            // お使いの種類(難易度)
    public int[] rewardRates;          // 褒賞の提供割合 [0] = Common, [1] = Uncommon, [2] = Rare
}
