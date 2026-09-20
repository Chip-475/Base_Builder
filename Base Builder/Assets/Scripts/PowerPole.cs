using UnityEngine;
using System;

public class PowerPole : InstalledObject
{
     public bool CanConnectTo(InstalledObject other)
    {
        if (other == null) return false;

        Cell[] myCells = GetCellsInBounds(bounds);
        Cell[] otherCells = other.GetCellsInBounds(other.GetBounds());

        foreach (var cell in myCells)
            foreach (var otherCell in otherCells)
                if (cell.Coords == otherCell.Coords)
                    return true;

        return false;
    }
// da fare una funzione che trova tutti  gli altri pali 

    public void Connect(InstalledObject other)
    {
        if (CanConnectTo(other))
        {
            Power = other.Power;
        }
    }

       private new void Start()
    {
        base.Start();

    }
}
