using System;
using System.Collections.Generic;
using UnityEngine;

public class PowerPole : Building
{
    [Serializable]
    public class PowerPoleConfig : Config
    {
        public int range;
    }

    public int Range { get; private set; }
    public Bounds ConnectionBounds { get; private set; }
    public bool isConnected;

    public PowerPole(PowerPole_View obj, PowerPoleConfig config) : base(obj, config)
    {

    }

    public bool CanConnectTo(Building_View other)
    {
        if (other == null) return false;
        ConnectionBounds = new Bounds(Object.transform.position, new Vector3(Range * 2, Range * 2, 0));
        Cell[] myCells = GetCellsInBounds(ConnectionBounds);
        var otherPole = other as PowerPole_View;
        if (otherPole != null)
        {
            Cell[] otherCells = otherPole.Building.GetCellsInBounds(otherPole.PowerPole.ConnectionBounds);

            foreach (var cell in myCells)
                foreach (var otherCell in otherCells)
                    if (cell.Coords == otherCell.Coords)
                        return true;
        }else
        {
            Cell[] otherCells = other.Building.GetCellsInBounds(other.Building.GetBounds());
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
        if (CanConnectTo(other.Object))
        {
            if(other.isConnected) isConnected = true;
            else if (isConnected)other.isConnected = true;
        }
    }

    //public void Connect(Machine other)
    //{
    //    if (CanConnectTo(other))
    //    {
    //        other.isConnected = true;
    //        if (isConnected) other.isPowered = true;
    //    }
    //}
}
