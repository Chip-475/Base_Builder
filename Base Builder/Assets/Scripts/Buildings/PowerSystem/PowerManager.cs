using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PowerNetworkDebug
{
    public NetworkManager network;
    public string id;
    public int buildings;
    public int poles;
    public int generators;
    public int machines;
    public List<string> connectedBuildings = new();
}

public class PowerManager : MonoBehaviour
{
    public static PowerManager instance;
    public GameObject powerObj;
    public static Dictionary<string, PowerPole> powerPolesDB=new();
    public static Dictionary<string,Generator> powerGeneratorDB=new();
    public static List<Machine> machineDB=new();

    [Header("Runtime Debug")]
    public int networkCount;
    public int registeredPoles;
    public int registeredGenerators;
    public int registeredMachines;
    public List<PowerNetworkDebug> networksDebug = new();
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
        network = building == null ? null : building.network;
        return network != null;
    }

    public void RebuildNetworks()
    {

        List<Building> buildings = new();
        foreach (PowerPole pole in powerPolesDB.Values)
            if (!buildings.Contains(pole)) buildings.Add(pole);
        foreach (Generator generator in powerGeneratorDB.Values)
            if (!buildings.Contains(generator)) buildings.Add(generator);
        foreach (Machine machine in machineDB)
            if (!buildings.Contains(machine)) buildings.Add(machine);

        foreach (NetworkManager network in powerObj.GetComponentsInChildren<NetworkManager>())
        {
            network.gameObject.SetActive(false);
            Destroy(network.gameObject);
        }
        foreach(Building building in buildings)
        {
            building.network = null;
        }
        List<Building> alrChecked = new();
        foreach (Building building in buildings)
        {
            if (alrChecked.Contains(building) || !(building is PowerPole)) continue;

            List<Building> connectedBuildings = GetConnectedBuildings(building, buildings, alrChecked);
            NetworkManager network = CreateNewNetwork();
            network.ConnectedBuildings.AddRange(connectedBuildings);
            foreach (Building connectedBuilding in connectedBuildings) connectedBuilding.network = network;
            network.RefreshPower();
            foreach (Building connectedBuilding in connectedBuildings)
                if (connectedBuilding is Machine machine && machine.requestedPower > 0)
                    machine.RefreshPowerRequest();
        }

        RefreshDebugInfo();
    }

    public void RefreshDebugInfo()
    {
        registeredPoles = powerPolesDB.Count;
        registeredGenerators = powerGeneratorDB.Count;
        registeredMachines = machineDB.Count;
        networksDebug.Clear();

        foreach (NetworkManager network in powerObj.GetComponentsInChildren<NetworkManager>())
        {
            PowerNetworkDebug debug = new();
            debug.network = network;
            debug.id = network.id;
            debug.buildings = network.ConnectedBuildings.Count;

            foreach (Building building in network.ConnectedBuildings)
            {
                debug.connectedBuildings.Add(building.SceneObj.name);
                if (building is PowerPole) debug.poles++;
                else if (building is Generator) debug.generators++;
                else if (building is Machine) debug.machines++;
            }

            networksDebug.Add(debug);
        }

        networkCount = networksDebug.Count;
    }

    [ContextMenu("Print Power Networks")]
    private void PrintNetworks()
    {
        RefreshDebugInfo();
        foreach (PowerNetworkDebug network in networksDebug)
            Debug.Log("Network " + network.id + ": " + string.Join(", ", network.connectedBuildings), this);
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
