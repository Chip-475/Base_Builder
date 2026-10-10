using UnityEngine;
using System.Collections.Generic;
using System.Linq;

#pragma warning disable
public class Tester : MonoBehaviour
{
    public static Tester Instance { get; private set; }

    public Vector3Int coords;

    private void Awake()
    {
        Instance = this;
    }
    /*
    [ContextMenu("Test")]
    public void WorldDatabaseTest()
    {
        if (WorldManager.World.Buildings[coords] is Machine)
            Debug.Log(true);
        else
            Debug.Log(false);
    }*/
}
