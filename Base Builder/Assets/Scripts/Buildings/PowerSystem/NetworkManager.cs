using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class NetworkManager : MonoBehaviour
{
    public string id;
    public List<Building> ConnectedBuildings { get; private set; } = new List<Building>();
    public float Generation { get; private set; }
    public float Consumption { get; private set; }
    public float Available { get { return Generation - Consumption; } }
    public bool IsNetworkRunning { get { return Generation > 0; } }
    public Color NetworkColor;
    [Header("Runtime Debug")]
    public int debugBuildings;
    public int debugPoles;
    public int debugGenerators;
    public int debugMachines;
    public List<GameObject> debugConnectedBuildings = new List<GameObject>();
    public List<LineRenderer> lineRenderers = new List<LineRenderer>();
    private Dictionary<Machine, float> machineRequests = new();

    private void Awake()
    {
        id = System.Guid.NewGuid().ToString();
        NetworkColor = PowerManager.networkColors.Pop();
    }
    public bool RequestPower(Machine machine, float amount)
    {
        if (amount <= 0) return false;

        float currentRequest;
        if (machineRequests.TryGetValue(machine, out float value)) currentRequest = value;
        else currentRequest = 0;
        float newConsumption = Consumption - currentRequest + amount;
        if (newConsumption > Generation) return false;

        machineRequests[machine] = amount;
        Consumption = newConsumption;
        UpdateDebugInfo();
        return true;
    }

    public void StopPowerRequest(Machine machine)
    {
        if (!machineRequests.TryGetValue(machine, out float amount)) return;

        Consumption -= amount;
        machineRequests.Remove(machine);
        UpdateDebugInfo();
    }

    public void RefreshPower()
    {
        Generation = 0;
        foreach (Building building in ConnectedBuildings)
            if (building is Generator generator && generator.Running)
                Generation += generator.Power;

        Consumption = 0;
        foreach (KeyValuePair<Machine, float> request in machineRequests) //KeyValuePair � la tupla chiave/valore contenuta nel dizionario
            Consumption += request.Value;

        UpdateDebugInfo();
    }

    public void UpdateDebugInfo()
    {
        debugBuildings = ConnectedBuildings.Count;
        debugPoles = 0;
        debugGenerators = 0;
        debugMachines = 0;
        debugConnectedBuildings.Clear();

        foreach (Building building in ConnectedBuildings)
        {
            debugConnectedBuildings.Add(building.SceneObj.gameObject);
            if (building is PowerPole) debugPoles++;
            else if (building is Generator) debugGenerators++;
            else if (building is Machine) debugMachines++;
        }

        if (PowerManager.instance != null) PowerManager.instance.RefreshDebugInfo();
    }
    public void startGenerator(Generator g, float w)
    {
        StartCoroutine(BurnFuel(g, w));
    }
    public IEnumerator BurnFuel(Generator generator, float waitingTime)
    {
        yield return null;
        if (!generator.Running) yield break;
        if (generator.inventory.Count == 0) { generator.SwitchState(false); yield break; }//to modify
        generator.SwitchState(true);
        yield return new WaitForSeconds(waitingTime);
        StartCoroutine(BurnFuel(generator, waitingTime));
    }
    public void shutDown()
    {
        foreach (Building building in ConnectedBuildings)
            if (building is Generator generator && generator.Running)
                generator.SwitchState(false);
    }
public void drawConnections()
    {
        foreach (LineRenderer line in lineRenderers)
        {
            Destroy(line.gameObject);
        }

        lineRenderers.Clear();

        Dictionary<PowerPole, int> connectionsCount = new();
        List<(PowerPole, PowerPole)> connections = new();

        foreach (Building building in ConnectedBuildings)
        {
            if (building is PowerPole pole)
            {
                pole.getConnectedBuildings();
                connectionsCount[pole] = 0;
            }
        }

        // Prima passata: garantisce almeno una connessione per palo
        foreach (Building building in ConnectedBuildings)
        {
            if (building is not PowerPole pole) continue;

            if (connectionsCount[pole] > 0) continue;

            foreach (Building other in pole.connectedBuildings
                .OfType<PowerPole>()
                .Where(other =>
                    other != pole &&
                    other.Network == pole.Network &&
                    connectionsCount.ContainsKey(other))
                .OrderBy(other =>
                    Vector2.Distance(
                        pole.SceneObj.transform.position,
                        other.SceneObj.transform.position
                    )))
            {
                PowerPole otherPP=other as PowerPole;
                if (connectionsCount[pole] >= 1) break;
                if (connectionsCount[otherPP] >= 3) continue;

                if (connections.Any(c =>
                    (c.Item1 == pole && c.Item2 == other) ||
                    (c.Item1 == other && c.Item2 == pole)))
                    continue;

                connections.Add((pole, otherPP));
                connectionsCount[pole]++;
                connectionsCount[otherPP]++;
                break;
            }
        }

        // Seconda passata: aggiunge connessioni extra ai vicini
        foreach (Building building in ConnectedBuildings)
        {
            if (building is not PowerPole pole) continue;

            foreach (Building other in pole.connectedBuildings
                .OfType<PowerPole>()
                .Where(other =>
                    other != pole &&
                    other.Network == pole.Network &&
                    connectionsCount.ContainsKey(other))
                .OrderBy(other =>
                    Vector2.Distance(
                        pole.SceneObj.transform.position,
                        other.SceneObj.transform.position
                    )))
            {
                PowerPole otherPP = other as PowerPole;
                if (connectionsCount[pole] >= 3) break;
                if (connectionsCount[otherPP] >= 3) continue;

                if (connections.Any(c =>
                    (c.Item1 == pole && c.Item2 == other) ||
                    (c.Item1 == other && c.Item2 == pole)))
                    continue;

                connections.Add((pole, otherPP));
                connectionsCount[pole]++;
                connectionsCount[otherPP]++;
            }
        }

        // Disegna ogni connessione una sola volta
        foreach (var connection in connections)
        {
            connection.Item1.drawConnection(
                connection.Item2.SceneObj.transform.position
            );
        }
    }
    public void OnDestroy()
    {
        foreach (LineRenderer line in lineRenderers)
        {
            Destroy(line.gameObject);
        }
        lineRenderers.Clear();
    }
}
