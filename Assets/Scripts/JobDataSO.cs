using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "JobDataSO", menuName = "Create JobDataSO")]
public class JobDataSO : ScriptableObject
{
    public List<JobData> jobDatasList = new List<JobData>();
}
