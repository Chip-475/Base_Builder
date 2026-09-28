using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;
public class NetworkManager : MonoBehaviour
{
    public string id;
    public List<Building> ConnectedBuildings { get; private set; }
    public float generation
    {
        get { return generation; }
        set { generation = value; if (generation > consuption) shutDown(); }
    }
    public float consuption;
    public float available
    {
        get { return generation-consuption; }
    }
    public bool isNetworkRunning
    {
        get { return (generation>0); }
    }
    public void Awake()
    {
        id=System.Guid.NewGuid().ToString();
    }
    public void shutDown()
    {
        foreach (Generator generator in ConnectedBuildings)
        {
            generator.running = false;
        }
    }
}
