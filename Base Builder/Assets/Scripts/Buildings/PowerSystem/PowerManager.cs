using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
public class PowerManager : MonoBehaviour
{
    public static PowerManager _instance;
    public static PowerManager instance
    {
        get
        {
            if (_instance == null) _instance = FindFirstObjectByType<PowerManager>();
            return _instance;
        }
    }
    public GameObject powerObj;
    public static Dictionary<string, PowerPole> powerPolesDB=new();
    public static Dictionary<string,Generator> powerGeneratorDB=new();
    public static List<Machine> machineDB=new();
    private void Awake()
    {
        _instance = this;
    }
    public bool checkNetwork(Building id, out NetworkManager network)
    {
        List<NetworkManager> networks = powerObj.GetComponentsInChildren<NetworkManager>().ToList();
        network = null;
        foreach (NetworkManager item in networks)
        {
            if (item.ConnectedBuildings.Contains(id))
            {
                network = item;
                return true;
            }
        }
        return false;
    }
    public void CreateNewNetwork()
    {
        GameObject network=Instantiate(new GameObject(),powerObj.transform);
        GameObject poles=Instantiate(new GameObject(),network.transform);
        GameObject generators=Instantiate(new GameObject(),network.transform);
        network.AddComponent<NetworkManager>();
    }
    public static PowerPole GetPowerPoleById(string id)
    {
        return powerPolesDB[id];
    }
    public static Generator GetGeneratorById(string id)
    {
        return powerGeneratorDB[id];
    }
    public static bool CanConnectTo( Building a,Building b)
    {
        if(!b.Data.connectsToPower||!a.Data.connectsToPower)return false;
        Bounds boundsA = a.Data.connectionBounds;
        boundsA.center = a.SceneObj.transform.position;
        Bounds boundsB = b.Data.connectionBounds;
        boundsB.center = b.SceneObj.transform.position;
        Cell[] myCells = Building.GetCellsInBounds(boundsA);
        Cell[] otherCells = Building.GetCellsInBounds(boundsB);

        foreach (var cell in myCells)
            foreach (var otherCell in otherCells)
                if (cell.Coords == otherCell.Coords)
                    return true;

        return false;
    }
}
