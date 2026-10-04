using UnityEngine;
using System.Collections.Generic;
public class PowerManager : MonoBehaviour
{
    public static PowerManager instance;
    public GameObject powerObj;
    public Dictionary<string, PowerPole> powerPolesDB=new();
    public Dictionary<string,Generator> powerGeneratorDB=new();
    public List<Machine> machineDB=new();
    private void Awake()
    {
        instance = this;
    }
    public void createNewNetwork()
    {
        GameObject network=Instantiate(new GameObject(),powerObj.transform);
        GameObject poles=Instantiate(new GameObject(),network.transform);
        GameObject generators=Instantiate(new GameObject(),network.transform);
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
