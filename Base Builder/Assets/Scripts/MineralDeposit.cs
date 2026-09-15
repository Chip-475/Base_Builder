using UnityEngine;
using System;

[Serializable]
public class MineralDeposit
{
    public MineralDeposit(Vector3Int coords, ResourceSO resource)
    {
        Coords = coords;
        Resource = resource;
    }

    public Vector3Int Coords { get; protected set; }
    public ResourceSO Resource { get; protected set; }
}
