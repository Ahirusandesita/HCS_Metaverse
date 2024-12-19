using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineIDManager : MonoBehaviour
{
    [SerializeField]
    private List<Machine> _machineList = new List<Machine>();

    public Machine GetMachine(int machineID)
    {
        for (int i = 0; i < _machineList.Count; i++)
        {
            if (_machineList[i].MachineID == machineID)
            {
                return _machineList[i];
            }
        }

        Debug.LogError($"<color=green>MachineID‚ªŒ©‚Â‚©‚è‚Ü‚¹‚ñ‚Å‚µ‚½</color>");
        return default;
    }
}
