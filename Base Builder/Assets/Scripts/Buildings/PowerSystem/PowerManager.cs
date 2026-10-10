using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/*
 * expected behavior:
 * everything in bounds of at least one powerpole is considered connected to that pole and in the same network
 *  a powerpole can show a max of 3 connections the others are just saved in the data
 *  this could be changed to show all connections but it would be a mess visually and a gameplay problem if set a cap to connections even in the data
 *  refer to electric view to see all the cell inside the network
 *  a fix could be to not show cables at all and rely on electric view
 *  generators and machines connects to only a powerpole
 *  if a generator or machine is in bounds of multiple powerpoles in different network it will choose only one of them,not merging the networks
 *  specify any other behavior that is expected and not specified here
 *  specify any other behavior that is not expected but happens in the current implementation during testing
 */
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
    public static Dictionary<string, PowerPole> powerPolesDB = new();
    public static Dictionary<string, Generator> powerGeneratorDB = new();
    public static List<Machine> machineDB = new();
    public static int networkCounter { get { return instance.powerObj.GetComponentsInChildren<NetworkManager>().Length; } }
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
        networkColors = new Stack<Color>(GenerateColors(1000, 1));
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
        //create a list with all the buildings
        List<Building> buildings = new();
        foreach (PowerPole pole in powerPolesDB.Values) if (!buildings.Contains(pole)) buildings.Add(pole);
        foreach (Generator generator in powerGeneratorDB.Values) if (!buildings.Contains(generator)) buildings.Add(generator);
        foreach (Machine machine in machineDB) if (!buildings.Contains(machine)) buildings.Add(machine);

        foreach (NetworkManager network in powerObj.GetComponentsInChildren<NetworkManager>()) //clear the old networks
        {
            network.gameObject.SetActive(false);
            networkColors.Push(network.NetworkColor);//rebuild the stack of colors so that the networks will maintain the same color after rebuild
            Destroy(network.gameObject);
        }
        foreach (Building building in buildings) building.Network = null; //reset the network of all buildings
        List<Building> alrChecked = new(); //create the list used for bts
        foreach (Building building in buildings)
        {
            if (alrChecked.Contains(building) || building is not PowerPole) continue; //skip already checked buildings and not powerpole as network generate from them

            List<Building> connectedBuildings = GetConnectedBuildings(building, buildings, alrChecked); //get all buildings connected to a pp if a is connected to b and b to c, c will be included in this list
            NetworkManager network = CreateNewNetwork();
            network.ConnectedBuildings.AddRange(connectedBuildings); //add all the buildings connected
            foreach (Building connectedBuilding in connectedBuildings) connectedBuilding.Network = network; //assign the network to all the connected building
            network.drawConnections(); //draw lines that show connections
            network.RefreshPower(); //refresh power production and consuption
            foreach (Building connectedBuilding in connectedBuildings)
                if (connectedBuilding is Machine machine && machine.RequestedPower > 0)
                    machine.RefreshPowerRequest(); //refresh the powerRequest sent from machines to the network
        }
        PowerSystemUI.instance.Draw(); //if electric view is enabled show cells inside the network
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
        //bts algo
        List<Building> connectedBuildings = new(); //return list of all buildings connected to the first
        Queue<Building> toCheck = new(); //queue of buildings discovered but not yet checked for connections
        toCheck.Enqueue(firstBuilding);
        visited.Add(firstBuilding); // list of buildings discovered not to check for connections again

        while (toCheck.Count > 0)
        {
            Building currentBuilding = toCheck.Dequeue(); //element to check
            connectedBuildings.Add(currentBuilding); //add itself to the list

            foreach (Building otherBuilding in buildings) //cycle between all the buildings existing
            {
                if (visited.Contains(otherBuilding)) continue; //skip if already checked
                if (!CanConnectTo(currentBuilding, otherBuilding)) continue; //skip if too far to enstablish a connection

                if (currentBuilding is not PowerPole && otherBuilding is PowerPole && connectedBuildings.Any(b => b is PowerPole)) //any checks for condition while contains checks only for existance, doesn't allow a network to exist without a pole
                    continue; //block connections between two not powerPole obj

                visited.Add(otherBuilding); //count as visited the building,it doesn't need to be cycled while checking other buildings connections
                toCheck.Enqueue(otherBuilding); //add to the list of item that will be current building in next cycle
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
        if (a == null || b == null) return false;
        if (a == b) return false;
        if (a is PowerPole pole) return pole.CanConnectTo(b);
        if (b is PowerPole otherPole) return otherPole.CanConnectTo(a);
        return false;
    }
    public Color[] GenerateColors(int amount, int seed)
    {
        System.Random random = new(seed);
        List<Color> colors = new();
        float hue = (float)random.NextDouble();
        for (int i = 0; i < amount; i++)
        {
            hue += random.Next(1, 10) * 0.001f;
            hue %= 1f;
            colors.Add(Color.HSVToRGB(hue, 0.5f, 0.95f));
            if (i > 1)
            {
                if (colors[i] == colors[i - 1])
                {
                    hue += 0.001f;
                    hue %= 1f;
                    colors[i] = Color.HSVToRGB(hue, 0.5f, 0.95f);
                }
            }
        }
        colors.Shuffle();
        return colors.ToArray();
    }
}
