/// <summary>
/// ���g���̎��(��Փx)�ɂ��J�܂̒񋟊����f�[�^
/// </summary>
[System.Serializable]
public class JobTypeRewardRatesData {
    public JobType jobType;            // ���g���̎��(��Փx)
    public int[] rewardRates;          // �J�܂̒񋟊��� [0] = Common, [1] = Uncommon, [2] = Rare
}
