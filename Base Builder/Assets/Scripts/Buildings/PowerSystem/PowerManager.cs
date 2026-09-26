using UnityEngine;
using System.Collections.Generic;
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
    public Dictionary<string, PowerPole> powerPolesDB=new();
    public Dictionary<string,Generator> powerGeneratorDB=new();
    public List<Machine> machineDB=new();
    private void Awake()
    {
        _instance = this;
    }
    public void createNewNetwork()
    {
        GameObject network=Instantiate(new GameObject(),powerObj.transform);
        network.transform.SetParent(powerObj.transform);
        GameObject poles=Instantiate(new GameObject(),network.transform);
        poles.transform.SetParent(network.transform);
        GameObject generators=Instantiate(new GameObject(),network.transform);
        generators.transform.SetParent(network.transform);
    }
    public static PowerPole GetPowerPoleById(string id)
    {
        return instance.powerPolesDB[id];
    }
    public static Generator GetGeneratorById(string id)
    {
        return instance.powerGeneratorDB[id];
    }
}
