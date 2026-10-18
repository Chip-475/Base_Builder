using UnityEngine;
using System.Collections.Generic;
public class alloyView :BuildingView
{
    public new alloyData data
    {
        get { return base.Data as alloyData;}
    }

    public alloyFurnace furnace
    {
        get {return Obj as alloyFurnace; }
    }

    void Awake()
    {
        if(data==null)
        {
            Debug.Log("non ce un alloy data");
            return;
        }
        Obj=new alloyFurnace(data,this);
    }

    void OnMouseDown()
    {
        if(MachineUI.Instance==null||data==null)return;
        MachineUI.Instance.Open(data.ricette);
    }
}
