using UnityEngine;
using System;

public class PowerPole : InstalledObject
{
     public bool canConnectTo(InstalledObject other)
    {
        if (other == null) return false;

        Cell[] myCells = GetCellsInBounds();
        Cell[] otherCells = other.GetCellsInBounds();

        foreach (var cell in myCells)
            foreach (var otherCell in otherCells)
                if (cell.Coords == otherCell.Coords)
                    return true;

        return false;
    }
// da fare una funzione che trova tutti  gli altri pali 

    public void connect(InstalledObject other)
    {
        if (canConnectTo(other))
        {
            Power = other.Power;
        }
    }

       private new void Start()
    {
        base.Start();

    }
}
