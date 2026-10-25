using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

public class BotManager : MonoBehaviour
{
    public static BotManager Instance { get; private set; }
    public static List<Bot> Bots { get; private set; } = new();

    // bots info
    public static int BotCount { get {  return Bots.Count; } }
    public static int EmployedBots { get { return Bots.Where(bot => bot.IsWorking).Count(); } }

    private void Awake()
    {
        Instance = this;
    }

    public static void RegisterBot(Bot bot)
    {
        if (Bots.Contains(bot))
        {
            Debug.Log($"{bot.Name} already registered.");
            return;
        }

        Bots.Add(bot);
    }
    public static void UnregisterBot(Bot bot)
    {
        if (!Bots.Contains(bot))
        {
            Debug.Log($"{bot.Name} not registered.");
            return;
        }

        Bots.Remove(bot);
    }

    public static Bot GetClosestAvailableBot(Building building)
    {
        List<Bot> bots = new(Bots.Where((bot) => bot.Occupied == false));
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
}
