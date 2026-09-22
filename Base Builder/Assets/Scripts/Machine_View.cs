using UnityEngine;

public class Machine_View : Building_View
{
    public Machine Machine { get; private set; }

    [SerializeField] Machine.MachineConfig config = new();

    void Start()
    {
        Machine = new(this, config);
    }
}
