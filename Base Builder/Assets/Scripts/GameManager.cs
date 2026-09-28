using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager _instance;
    public static GameManager Instance 
    { 
        get
        {
            if (_instance = null) _instance = FindFirstObjectByType<GameManager>();
            return _instance;
        }
    }

    [SerializeField] Dictionary<string, Bot> bots = new();

    void Awake()
    {
        _instance = this;
    }

    public static Bot GetBotById(string id) { return Instance.bots[id]; }
    public static void SetBot(string id, Bot bot) { Instance.bots[id] = bot; }
}
