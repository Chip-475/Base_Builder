using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] Dictionary<string, Bot> bots = new();

    void Awake()
    {
        Instance = this;
    }

    public static Bot GetBotById(string id) { return Instance.bots[id]; }
    public static void SetBot(string id, Bot bot) { Instance.bots[id] = bot; }
}
