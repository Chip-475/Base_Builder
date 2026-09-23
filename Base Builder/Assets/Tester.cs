using UnityEngine;
using System.Collections.Generic;

public class Tester : MonoBehaviour
{
    public static Tester Instance { get; private set; }

    public List<Machine> machines = new();

    private void Awake()
    {
        Instance = this;
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
