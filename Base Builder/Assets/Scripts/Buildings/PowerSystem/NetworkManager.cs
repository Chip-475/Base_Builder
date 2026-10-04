using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class NetworkManager : MonoBehaviour
{
    public string id;
    [field:SerializeField]public List<Building> ConnectedBuildings { get; private set; } = new();
    [field: SerializeField] public float Generation { get; private set; }
    [field: SerializeField] public float Consumption { get; private set; }
    public float Available { get { return Generation - Consumption; } }
    public bool IsNetworkRunning { get { return Generation > 0; } }

    private Dictionary<Machine, float> machineRequests = new();

    private void Awake()
    {
        id=System.Guid.NewGuid().ToString();
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
        return true;
    }

    public void StopPowerRequest(Machine machine)
    {
        if (!machineRequests.TryGetValue(machine, out float amount)) return;

        Consumption -= amount;
        machineRequests.Remove(machine);
    }

    public void RefreshPower()
    {
        Generation = 0;
        foreach (Building building in ConnectedBuildings)
            if (building is Generator generator && generator.running)
                Generation += generator.Power;

        Consumption = 0;
        foreach (KeyValuePair<Machine, float> request in machineRequests) //KeyValuePair è la tupla chiave/valore contenuta nel dizionario
            Consumption += request.Value;
    }
    public void startGenerator(Generator g,float w)
    {
        StartCoroutine(BurnFuel(g, w));
    }
    public IEnumerator BurnFuel(Generator generator,float waitingTime)
    {
        yield return null;
        if (!generator.running) yield break;
        if (generator.inventory.Count == 0) { generator.SwitchState(false);yield break; }//to modify
        generator.SwitchState(true);
        yield return new WaitForSeconds(waitingTime);
        StartCoroutine(BurnFuel(generator, waitingTime));
    }
    public void shutDown()
    {
        foreach (Building building in ConnectedBuildings)
            if (building is Generator generator && generator.running)
                generator.SwitchState(false);
    }
}
