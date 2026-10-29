using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerBot : Bot
{
    public class Task
    {
        public Task(WorkerBot bot, Machine workplace, RecipeSO recipe, int amount)
        {
            Bot = bot;
            Workplace = workplace;
            Recipe = recipe;
            Amount = amount;
        }

        public WorkerBot Bot { get; private set; }
        public Machine Workplace { get; private set; }
        public RecipeSO Recipe { get; private set; }
        public int Amount { get; private set; }

        public bool IsValid()
        {
            return Bot != null
                && Workplace != null
                && Workplace.Data != null
                && Workplace.Data.usableRecipes != null
                && Recipe != null
                && Workplace.Data.usableRecipes.Contains(Recipe)
                && Amount > 0;
        }
    }

    public Task CurrentTask { get; set; }
    public bool HasMaterials { get; private set; }

    LogisticsManager.LogisticsRequest materialRequest;
    LogisticsManager.LogisticsRequest depositRequest;
    Task materialsTask;
    Dictionary<ResourceSO, int> requiredMaterials;

    public WorkerBot(BotView view, BotData data, BotConfig config) : base(view, data, config)
    {
        WorkRoutine = SceneObj.StartCoroutine(Pipeline());
    }

    public override IEnumerator Pipeline()
    {
        while (true)
        {
            if (CurrentTask == null)
            {
                yield return null;
                continue;
            }

            if (!CurrentTask.IsValid())
            {
                Fail("Worker task is invalid.");
                yield return null;
                continue;
            }

            if (depositRequest != null)
            {
                if (LogisticsManager.Requests.Contains(depositRequest))
                {
                    yield return null;
                    continue;
                }

                CompleteTask();
                yield return null;
                continue;
            }

            if (!CanReachWorkplace(CurrentTask.Workplace))
            {
                Fail("Cannot pathfind to workplace.");
                yield return null;
                continue;
            }

            Dictionary<ResourceSO, int> materials = GetRequiredMaterials(CurrentTask);
            if (!MaterialsAvailableOrRequested(materials))
            {
                Fail("Depot does not have the required materials.");
                yield return null;
                continue;
            }

            if (!AtWorkPoint(CurrentTask.Workplace))
            {
                if (!MoveToWorkplace(CurrentTask.Workplace))
                    Fail("Cannot pathfind to workplace.");

                yield return null;
                continue;
            }

            if (!Inventory.Contains(materials))
            {
                if (materialRequest == null)
                    IssueMaterialRequest(materials);

                yield return null;
                continue;
            }

            if (!HasMaterials)
            {
                Inventory.RemoveItems(materials);
                HasMaterials = true;

                yield return new WaitForSeconds(CurrentTask.Recipe.completionTime);

                if (!AddOutputs())
                {
                    Fail("Worker inventory cannot hold the task outputs.");
                    yield return null;
                    continue;
                }

                HasMaterials = false;
                IssueDepositRequest();
                yield return null;
                continue;
            }

            CompleteTask();
            yield return null;
        }
    }

    bool CanReachWorkplace(Machine workplace)
    {
        return Pathfinder.CanPathfind(
            Coords.ToVector3Int(),
            GetWorkPoint(workplace).ToVector3Int());
    }

    Dictionary<ResourceSO, int> GetRequiredMaterials(Task task)
    {
        if (materialsTask == task)
            return requiredMaterials;

        requiredMaterials = new();
        foreach (var material in task.Recipe.inputs)
            requiredMaterials[material.Key] = material.Value * task.Amount;

        materialsTask = task;
        return requiredMaterials;
    }

    bool MaterialsAvailableOrRequested(Dictionary<ResourceSO, int> materials)
    {
        if (Inventory.Contains(materials) || Depot.Inventory.Contains(materials))
            return true;

        if (materialRequest != null && LogisticsManager.Requests.Contains(materialRequest))
            return true;

        materialRequest = null;
        return false;
    }

    void IssueMaterialRequest(Dictionary<ResourceSO, int> materials)
    {
        Depot source = FindDepotWithMaterials(materials, GetWorkPoint(CurrentTask.Workplace));
        if (source == null)
        {
            Fail("No depot can supply the required materials.");
            return;
        }

        materialRequest = new LogisticsManager.LogisticsRequest(
            this,
            new Dictionary<ResourceSO, int>(materials),
            Depot.Inventory,
            source.Coords,
            Inventory,
            GetWorkPoint(CurrentTask.Workplace),
            priority: 1);
        LogisticsManager.Requests.Add(materialRequest);
    }

    bool AddOutputs()
    {
        Dictionary<ResourceSO, int> outputs = new();
        foreach (var output in CurrentTask.Recipe.outputs)
            outputs[output.Key] = output.Value * CurrentTask.Amount;

        if (!Inventory.CanAddItems(outputs))
            return false;

        Inventory.AddItems(outputs);
        return true;
    }

    void IssueDepositRequest()
    {
        if (Inventory.IsEmpty)
        {
            CompleteTask();
            return;
        }

        Depot destinationDepot = FindDepotForMaterials(Inventory.Items, Coords);
        if (destinationDepot == null)
        {
            Fail("No depot can receive the task outputs.");
            return;
        }

        depositRequest = new LogisticsManager.LogisticsRequest(
            this,
            new Dictionary<ResourceSO, int>(Inventory.Items),
            Inventory,
            Coords,
            Depot.Inventory,
            destinationDepot.Coords,
            priority: 1);
        LogisticsManager.Requests.Add(depositRequest);
    }

    Depot FindDepotWithMaterials(Dictionary<ResourceSO, int> materials, Vector3 position)
    {
        Depot closest = null;
        int closestDistance = int.MaxValue;

        foreach (var depot in BuildingManager.GetBuildingsOfType<Depot>())
        {
            if (depot.Occupied || !Depot.Inventory.Contains(materials))
                continue;

            int distance = (depot.Coords - position.ToVector3Int()).sqrMagnitude;
            if (distance >= closestDistance)
                continue;

            closest = depot;
            closestDistance = distance;
        }

        return closest;
    }

    Depot FindDepotForMaterials(Dictionary<ResourceSO, int> materials, Vector3 position)
    {
        foreach (var depot in BuildingManager.GetBuildingsOfType<Depot>())
            if (!depot.Occupied && Depot.Inventory.CanAddItems(materials))
                return depot;

        return null;
    }

    void CompleteTask()
    {
        CurrentTask = null;
        materialsTask = null;
        requiredMaterials = null;
        materialRequest = null;
        depositRequest = null;
        HasMaterials = false;
        destination = null;
        Path.Clear();
    }

    new void Fail(string reason)
    {
        if (materialRequest != null)
            LogisticsManager.Requests.Remove(materialRequest);
        if (depositRequest != null)
            LogisticsManager.Requests.Remove(depositRequest);

        materialRequest = null;
        depositRequest = null;
        CurrentTask = null;
        materialsTask = null;
        requiredMaterials = null;
        HasMaterials = false;
        destination = null;
        Path.Clear();
        Debug.Log(reason);
    }
}
