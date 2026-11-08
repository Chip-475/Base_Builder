using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BotManager : MonoBehaviour
{
    [Serializable]
    public class BotUpgrade
    {
        public BotData data;
        [Space]
        public float deltaSpeed;
        public int deltaMaxWeight;
        public int deltaMiningLevel;
        [Tooltip("Negative value.")] public float deltaPowerConsumption;
        public int deltaMaxPower;
    }

    public static BotManager Instance { get; private set; }
    public static List<Bot> Bots { get; private set; } = new();

    [SerializeField] List<BotData> botDatas = new();

    // bots info
    public static int BotCount { get {  return Bots.Count; } }
    public static int EmployedBots { get { return Bots.Count(bot => bot != null && bot.Occupied); } }

    private void Awake()
    {
        Instance = this;
    }

    public static void RegisterBot(Bot bot)
    {
        if (bot == null)
            return;

        if (Bots.Contains(bot))
        {
            Debug.Log($"{bot.Name} already registered.");
            return;
        }

        Bots.Add(bot);
    }
    public static void UnregisterBot(Bot bot)
    {
        Bots.Remove(bot);
    }

    public static void ApplyUpgrade(BotData data)
    {
        if (data == null)
            return;

        foreach (Bot bot in Bots)
            if (bot != null && bot.Data == data)
                bot.RefreshDataUpgrades();
    }

    public static Bot GetClosestAvailableBot(Building building)
    {
        if (building == null || building.SceneObj == null)
            return null;

        List<Bot> bots = new(Bots.Where(bot => bot != null && bot.SceneObj != null && !bot.Occupied && !bot.PickedUp));
        if(bots.Count == 0)
        {
            Debug.Log("No available bots.");
            return null;
        }

        Bot closest = bots[0];
        foreach (Bot bot in bots)
        {
            float closestDis = Vector3.Distance(closest.SceneObj.transform.position, building.SceneObj.transform.position);
            float nextDis = Vector3.Distance(bot.SceneObj.transform.position, building.SceneObj.transform.position);

            if(nextDis < closestDis)
                closest = bot;
        }

        return closest;
    }
    public static T GetClosestAvailableBot<T>(Building building)
        where T : Bot
    {
        if (building == null || building.SceneObj == null)
            return null;

        List<T> bots = new(Bots.OfType<T>().Where(bot => bot != null && bot.SceneObj != null && !bot.Occupied && !bot.PickedUp));
        if (bots.Count == 0)
        {
            Debug.Log("No available bots.");
            return null;
        }

        T closest = bots[0];
        foreach (T bot in bots)
        {
            float closestDis = Vector3.Distance(closest.SceneObj.transform.position, building.SceneObj.transform.position);
            float nextDis = Vector3.Distance(bot.SceneObj.transform.position, building.SceneObj.transform.position);

            if (nextDis < closestDis)
                closest = bot;
        }

        return closest;
    }
}
