using UnityEngine;
using System.Collections.Generic;

#pragma warning disable
public class Tester : MonoBehaviour
{
    public static Tester _instance;
    public static Tester Instance
    {
        get
        {
            if(_instance==null)_instance=FindFirstObjectByType<Tester>();
            return _instance;
        }
    }

    public List<Machine> machines = new();

    private void Awake()
    {
        _instance = this;
    }

    [ContextMenu("Print")]
    public void PrintMachines()
    {
        foreach(var machine in machines)
        {
            Debug.Log(machine.SceneObj.GetType().ToString());
        }
    }
}
