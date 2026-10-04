using System.Collections.Generic;
using UnityEngine;
public class PowerManager : MonoBehaviour
{
    public static PowerManager instance;
    public GameObject powerObj;
    public static Dictionary<string, PowerPole> powerPolesDB=new();
    public static Dictionary<string,Generator> powerGeneratorDB=new();
    public static List<Machine> machineDB=new();
    private void Awake()
    {
        instance = this;
        if (powerObj == null) powerObj = gameObject;
        powerPolesDB.Clear();
        powerGeneratorDB.Clear();
        machineDB.Clear();
    }
    public void RegisterBuilding()
    {
        RebuildNetworks();
    }

    public void UnregisterBuilding()
    {
        RebuildNetworks();
    }

    public bool checkNetwork(Building building, out NetworkManager network)
    {
        foreach (NetworkManager item in powerObj.GetComponentsInChildren<NetworkManager>())
            if (item.ConnectedBuildings.Contains(building))
            {
                network = item;
                return true;
            }

        network = null;
        return false;
    }

    public void RebuildNetworks()
    {

        List<Building> buildings = new();
        buildings.AddRange(powerPolesDB.Values);
        buildings.AddRange(powerGeneratorDB.Values);
        buildings.AddRange(machineDB);

        foreach (NetworkManager network in powerObj.GetComponentsInChildren<NetworkManager>())
        {
            Destroy(network.gameObject);
        }

        List<Building> alrChecked = new();
        foreach (Building building in buildings)
        {
            if (alrChecked.Contains(building)) continue;

            List<Building> connectedBuildings = GetConnectedBuildings(building, buildings, alrChecked);
            NetworkManager network = CreateNewNetwork();
            network.ConnectedBuildings.AddRange(connectedBuildings);
            network.RefreshPower();
            foreach (Building connectedBuilding in connectedBuildings)
                if (connectedBuilding is Machine machine && machine.requestedPower > 0)
                    machine.RefreshPowerRequest();
        }
    }

    private List<Building> GetConnectedBuildings(Building firstBuilding, List<Building> buildings, List<Building> visited)
    {
        List<Building> connectedBuildings = new();
        List<Building> toCheck = new();
        toCheck.Add(firstBuilding);
        visited.Add(firstBuilding);

        while (toCheck.Count > 0)
        {
            Building currentBuilding = toCheck[0];
            toCheck.RemoveAt(0);
            connectedBuildings.Add(currentBuilding);

            foreach (Building otherBuilding in buildings)
            {
                if (visited.Contains(otherBuilding) || !CanConnectTo(currentBuilding, otherBuilding)) continue;

                visited.Add(otherBuilding);
                toCheck.Add(otherBuilding);
            }
        }

        return connectedBuildings;
    }

    public NetworkManager CreateNewNetwork()
    {
        GameObject networkObject = new GameObject("Network");
        networkObject.transform.SetParent(powerObj.transform);
        return networkObject.AddComponent<NetworkManager>();
    }
    public static PowerPole GetPowerPoleById(string id)
    {
        return powerPolesDB[id];
    }
    public static Generator GetGeneratorById(string id)
    {
        return powerGeneratorDB[id];
    }
    public static bool CanConnectTo(Building a, Building b)
    {
        if(a == null || b == null) return false;
        if (a == b) return false;
        if (a is PowerPole pole) return pole.CanConnectTo(b);
        if (b is PowerPole otherPole) return otherPole.CanConnectTo(a);
        return false;
    }
}
