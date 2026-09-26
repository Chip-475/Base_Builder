using System;
using System.Collections.Generic;
using UnityEngine;

public class PowerPole : Building
{
    public new PowerPoleData Data => base.Data as PowerPoleData;
    public new PowerPoleView SceneObj => base.SceneObj as PowerPoleView;

    public Bounds ConnectionBounds { get; private set; }
    public bool isConnected;
    public string poleId;
    public Action<PowerPole> OnDestroy;
    public PowerPole(PowerPoleData data, PowerPoleView sceneObj) : base(data, sceneObj)
    {
        ConnectionBounds = new Bounds(SceneObj.transform.position, new Vector3(Data.range * 2, Data.range * 2, 0));
        poleId=System.Guid.NewGuid().ToString();
        PowerManager.instance.powerPolesDB[poleId]= this;
    }

    public bool CanConnectTo(Building other)
    {
        if (other == null) return false;
        
        Cell[] myCells = GetCellsInBounds(ConnectionBounds);
        var otherPole = other as PowerPole;
        if (otherPole != null)
        {
            Cell[] otherCells = otherPole.GetCellsInBounds(otherPole.ConnectionBounds);

            foreach (var cell in myCells)
                foreach (var otherCell in otherCells)
                    if (cell.Coords == otherCell.Coords)
                        return true;
        }
        else
        {
            Cell[] otherCells = other.GetCellsInBounds(other.GetBounds());
            foreach (var cell in myCells)
                foreach (var otherCell in otherCells)
                    if (cell.Coords == otherCell.Coords)
                        return true;
        }
        return false;
    }
    public void ConnectToPole(PowerPole other)
    {
        //the ship will set the first isConnected to the poles directly connected to it
        if (CanConnectTo(other))
        {
            if (other.isConnected) isConnected = true;
            else if (isConnected) other.isConnected = true;
        }
    }
    public void 

    //public void Connect(Machine other)
    //{
    //    if (CanConnectTo(other))
    //    {
    //        other.isConnected = true;
    //        if (isConnected) other.isPowered = true;
    //    }
    //}
}