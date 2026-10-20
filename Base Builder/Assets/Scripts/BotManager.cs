using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

public class BotManager : MonoBehaviour
{
    public struct WorkRequest
    {
        public WorkRequest(Bot bot, Building building, RecipeSO recipe)
        {
            this.bot = bot;
            this.recipe = recipe;
            
            this.building = building;
            iWorkplace = building as IWorkplace;
        }

        public Bot bot;
        public RecipeSO recipe;

        public Building building;
        public IWorkplace iWorkplace;
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

    public IEnumerator ProcessRequest(WorkRequest request)
    {
        if (!IsRequestValid(request))
        {
            Debug.Log("Request is invalid.");
            yield break;
        }

        if(!request.iWorkplace.HasRequiredItems(request))
        {
            foreach (var kvp in request.recipe.inputs)
            {
                if (!Depot.Inventory.Items.ContainsKey(kvp.Key))
                    yield break;
                if (Depot.Inventory.Items[kvp.Key] < kvp.Value)
                    yield break;
            }

            yield return StartCoroutine(Fetch(request));
        }

        yield return StartCoroutine(Work(request));
    }
    public IEnumerator Work(WorkRequest request)
    {
        request.bot.Occupied = true;

        request.bot.BotView.SetDestination(request.building.SceneObj.BotWorkPoint.transform.position.ToVector3Int());
        request.bot.BotView.StartMoving();
        yield return new WaitUntil(() => Vector3.Distance(
            request.bot.BotView.transform.position,
            request.building.SceneObj.transform.position) < 0.05f);
        StartCoroutine(request.iWorkplace.Work(request));
        yield return new WaitForSeconds(request.recipe.completionTime);

        request.bot.Occupied = false;
    }
    public IEnumerator Fetch(WorkRequest request)
    {
        request.bot.Occupied = true;

        request.bot.BotView.SetDestination(
            BuildingManager.GetClosestAvailableBuilding(
                request.bot.Coords.ToVector3Int(),
                typeof(Building)).SceneObj.BotWorkPoint.transform.position.ToVector3Int());

        request.bot.BotView.StartMoving();
        yield return new WaitUntil(() => Vector3.Distance(
            request.bot.BotView.transform.position,
            request.building.SceneObj.transform.position) < 0.05f);
        
        foreach (var kvp in request.recipe.inputs)
        {
            Depot.Inventory.Remove(kvp.Key, kvp.Value);
            request.bot.Inventory.AddItem(kvp.Key, kvp.Value);
        }

        request.bot.Occupied = false;
    }

    public bool IsRequestValid(WorkRequest request)
    {
        Pathfinder.Pathfind(request.bot.Coords.ToVector3Int(), request.building.SceneObj.BotWorkPoint.transform.position.ToVector3Int(), out List<Vector3Int> path);

        if (request.bot.Occupied)
        {
            Debug.Log("Bot is not available.");
            return false;
        }
        if (request.building.Occupied)
        {
            Debug.Log("Building is occupied.");
            return false;
        }
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
