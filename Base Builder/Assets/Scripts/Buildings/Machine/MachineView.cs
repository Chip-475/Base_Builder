using System.Collections.Generic;
using UnityEngine;

public class MachineView : BuildingView
{
    [Header("test")]
    public RecipeSO test;
    public new MachineData Data => base.Data as MachineData;
    public new Machine Obj
    {
        get { return base.Obj as Machine; }
        set { base.Obj = value; }
    }

    private void Start()
    {
        InitializeMachine();
    }

    void OnMouseDown()
    {
        // A machine instantiated from BuildMode can receive its first mouse
        // event before Unity has called Start().
        InitializeMachine();
    }

    private void InitializeMachine()
    {
        if (Obj != null)
            return;

        if (Data == null)
        {
            Debug.LogError($"Machine '{name}' has no MachineData assigned.", this);
            return;
        }

        Obj = new Machine(Data, this);
    }

    //test 
    private void testInput()
    {
        InitializeMachine();
        if (Obj == null || test == null) return;
        foreach(KeyValuePair<ResourceSO,int> ric in test.inputs)
        {
            Obj.aggIn(ric.Key, ric.Value);
        }
    }
}
