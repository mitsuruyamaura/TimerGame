using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���g���̎��(��Փx)�ɂ��J�܂̒񋟊����f�[�^�x�[�X
/// </summary>
[CreateAssetMenu(fileName = "JobTypeRewardRatesDataSO", menuName = "Create JobTypeRewardRatesDataSO")]
public class JobTypeRewardRatesDataSO : ScriptableObject {

    public List<JobTypeRewardRatesData> jobTypeRewardRatesDataList = new List<JobTypeRewardRatesData>();
}
