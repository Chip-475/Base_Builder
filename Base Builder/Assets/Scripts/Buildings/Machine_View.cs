using System.Reflection.PortableExecutable;
using Unity.VisualScripting;
using UnityEngine;

public class Machine_View : Building_View
{
    public Machine Machine => Building as Machine;

    [SerializeField] Machine.MachineConfig config = new();

    new void Start()
    {
        Building = new(this, config);
    }
}
