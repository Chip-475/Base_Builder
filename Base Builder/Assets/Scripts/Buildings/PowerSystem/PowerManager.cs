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
    public static int networkCounter { get { return instance.powerObj.GetComponentsInChildren<NetworkManager>().Length; } }
    //public static Stack<Color> networkColors = new Stack<Color>(new Color[] { Color.red, Color.green, Color.gray, Color.blue, Color.yellow, Color.cyan, Color.magenta, Color.white, Color.tan, Color.coral, Color.aliceBlue, Color.azure });
    public static Stack<Color> networkColors = new Stack<Color>();
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
        networkColors = new Stack<Color>(GenerateColors(1000,1));
    }
    public void RegisterBuilding()
    {
        RebuildNetworks();
    }

    public void UnregisterBuilding()
    {
        RebuildNetworks();
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
            networkColors.Push(network.NetworkColor);
            Destroy(network.gameObject);
        }
        foreach (Building building in buildings)
        {
            building.Network = null;
        }
        List<Building> alrChecked = new();
        foreach (Building building in buildings)
        {
            if (alrChecked.Contains(building) || building is not PowerPole) continue;

            List<Building> connectedBuildings = GetConnectedBuildings(building, buildings, alrChecked);
            NetworkManager network = CreateNewNetwork();
            network.ConnectedBuildings.AddRange(connectedBuildings);
            foreach (Building connectedBuilding in connectedBuildings) connectedBuilding.Network = network;
            network.drawConnections();
            network.RefreshPower();
            foreach (Building connectedBuilding in connectedBuildings)
                if (connectedBuilding is Machine machine && machine.RequestedPower > 0)
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
            PowerNetworkDebug debug = new()
            {
                network = network,
                id = network.id,
                buildings = network.ConnectedBuildings.Count
            };

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

    private List<Building> GetConnectedBuildings(Building firstBuilding, List<Building> buildings, List<Building> visited)
    {
        List<Building> connectedBuildings = new();
        List<Building> toCheck = new()
        {
            firstBuilding
        };
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
        GameObject networkObject = new("Network");
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
    public Color[] GenerateColors(int amount,int seed)
    {
        System.Random random = new(seed);
        Color[] colors = new Color[amount];
        float hue=(float)random.NextDouble();
        for (int i = 0; i < amount; i++)
        {
            hue += i/10;
            hue %= 1f;
            colors[i] = Color.HSVToRGB(hue, 0.5f, 0.95f);
            if (i > 1)
            {
                if (colors[i] == colors[i-1])
                {
                    hue += 0.1f;
                    hue %= 1f;
                    colors[i] = Color.HSVToRGB(hue, 0.5f, 0.95f);
                }
            }
        }
        return colors;
    }
}
