using System;
using System.Collections.Generic;
using UnityEngine;

public class PowerPole : InstalledObject
{
    public int poleRadius;
    public Bounds b;
    public bool CanConnectTo(InstalledObject other)
    {
        if (other == null) return false;
        Debug.Log("dio");
        b=new Bounds(transform.position, new Vector3(poleRadius * 2, poleRadius * 2, 0));
        Cell[] myCells = GetCellsInBounds(b);
        PowerPole otherPole = other as PowerPole;
        Cell[] otherCells = otherPole.GetCellsInBounds(otherPole.b);

        foreach (var cell in myCells)
            foreach (var otherCell in otherCells)
                if (cell.Coords == otherCell.Coords)
                    return true;

        return false;
    }

    // da fare una funzione che trova tutti  gli altri pali 

    public void Connect(PowerPole other)
    {
        if (CanConnectTo(other))
        {
            if(other.Power>0)
            Power = other.Power;
            else
            other.Power = Power;
        }
    }

    private new void Start()
    {
        poleManager.instance.Start();
        base.Start();
    }
    private void OnDestroy()
    {
        poleManager.instance.Start();
    }
    private void Update()
    {
        if (Power <= 0)
        {
            gameObject.GetComponent<SpriteRenderer>().color = Color.red;
        }
        else
        {
            gameObject.GetComponent<SpriteRenderer>().color = Color.white;
        }
    }
    new void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Bounds bounds = b;
        Debug.Log(bounds.size);
        Vector3 min = bounds.min;
        Vector3 max = bounds.max;

        Gizmos.DrawLine(new Vector3(min.x, min.y, 0), new Vector3(max.x, min.y, 0));
        Gizmos.DrawLine(new Vector3(max.x, min.y, 0), new Vector3(max.x, max.y, 0));
        Gizmos.DrawLine(new Vector3(max.x, max.y, 0), new Vector3(min.x, max.y, 0));
        Gizmos.DrawLine(new Vector3(min.x, max.y, 0), new Vector3(min.x, min.y, 0));
    }
}
