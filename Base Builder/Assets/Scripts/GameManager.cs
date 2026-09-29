using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager _instance;
    public static GameManager Instance 
    { 
        get
        {
            if (_instance == null) _instance = FindFirstObjectByType<GameManager>();
            return _instance;
        }
    }

    [SerializeField] List<Bot> bots = new List<Bot>();

    void Awake()
    {
        _instance = this;
    }

    public static Bot GetBotById(int i) { return Instance.bots[i]; }
    public static void SetBot(Bot bot) { Instance.bots.Add(bot); }
}
