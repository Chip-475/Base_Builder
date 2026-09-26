using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public string id;
    public string[] generatorIds;
    public string[] polesIds;
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
        foreach (string generator in generatorIds)
        {
            PowerManager.GetGeneratorById(generator).running = false;
        }
    }
}
