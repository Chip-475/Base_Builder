using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public string id;
    public string[] generatorIds;
    public string[] polesIds;
    public NetworkManager(string id)
    {
        this.id = id;
    }
}
