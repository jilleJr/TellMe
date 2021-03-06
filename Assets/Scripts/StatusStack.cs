using System.Collections.Generic;
using UnityEngine;

public class StatusStack : MonoBehaviour
{
    public List<StepStatus> statuses = new();
    public GameObject prefabStepStatus;

    public StepStatus AddStatus(string message)
    {
        var clone = Instantiate(prefabStepStatus, transform);
        var status = clone.GetComponentInChildren<StepStatus>();
        status.textMessage.text = message;
        statuses.Add(status);
        return status;
    }

    public void ClearStatuses()
    {
        foreach (var status in statuses)
        {
            Destroy(status.gameObject);
        }
        statuses.Clear();
    }
}
