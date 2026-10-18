using UnityEngine;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Linq;

public class BotManager : MonoBehaviour
{
    public struct WorkRequest
    {
        public WorkRequest(Bot bot, Building building, RecipeSO recipe)
        {
            this.bot = bot;
            this.building = building;
            this.recipe = recipe;
        }

        public Bot bot;
        public Building building;
        public RecipeSO recipe;
    }

    public static BotManager Instance { get; private set; }
    public static List<Bot> Bots { get; private set; } = new();

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

    public static void ProcessRequest(WorkRequest request)
    {
        if (!IsRequestValid(request))
        {
            Debug.Log("Request is invalid.");
            Debug.Log($@"");
        }

        request.bot.Occupied = true;
        request.bot.BotView.SetDestination(request.building.SceneObj.BotWorkPoint.transform.position.ToVector3Int());
        request.bot.BotView.StartMoving();
    }
    public static bool IsRequestValid(WorkRequest request)
    {
        if (request.bot.Occupied)
        {
            Debug.Log("Bot is not available.");
            return false;
        }
        Pathfinder.Pathfind(request.bot.Coords.ToVector3Int(), request.building.SceneObj.BotWorkPoint.transform.position.ToVector3Int(), out List<Vector3Int> path);
        if (path.Count == 0)
        {
            Debug.Log("Cannot pathfind to target location.");
            return false;
        }

        return true;
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
            float closestDis = Vector3.Distance(closest.BotView.transform.position, building.SceneObj.transform.position);
            float nextDis = Vector3.Distance(bot.BotView.transform.position, building.SceneObj.transform.position);

            if(nextDis < closestDis)
                closest = bot;
        }

        return closest;
    }
}
