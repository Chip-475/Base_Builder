using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
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

        public WorkerBot Bot { get; }
        public Machine Workplace { get; }
        public RecipeSO Recipe { get; }
        public int Amount { get; }

        public bool IsValid()
        {
            return Bot != null && Workplace != null && Workplace.Data != null
                && Workplace.Data.usableRecipes != null && Recipe != null
                && Recipe.inputs != null && Recipe.outputs != null
                && Workplace.Data.usableRecipes.Contains(Recipe)
                && Workplace.CanUseWorker(Bot) && Amount > 0
                && Recipe.inputs.Count > 0 && Recipe.outputs.Count > 0
                && Recipe.inputs.All(item => item.Key != null && item.Value > 0)
                && Recipe.outputs.All(item => item.Key != null && item.Value > 0);
        }
    }

    Task currentTask;
    int completedAmount;
    bool waitingForMachineBatch;
    int expectedCompletedBatches;
    public Task CurrentTask
    {
        get => currentTask;
        set
        {
            if (ReferenceEquals(currentTask, value))
            {
                if (value != null && !Occupied)
                    StartWorking();
                return;
            }

            if (value != null && Occupied)
                return;

            currentTask?.Workplace?.ReleaseWorker(this);
            currentTask = value;
            completedAmount = 0;
            waitingForMachineBatch = false;
            expectedCompletedBatches = 0;

            if (value == null)
            {
                StopWorking();
                destination = null;
                Path.Clear();
                return;
            }

            if (!value.Workplace.TryAssignWorker(this, value.Recipe))
            {
                currentTask = null;
                Debug.LogWarning("Could not assign the worker to that machine.");
                return;
            }

            StartWorking();
        }
    }

    public WorkerBot(BotView view, BotData data, BotConfig config) : base(view, data, config)
    {
    }

    public override async UniTask Pipeline(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested && CurrentTask != null)
        {
            Task task = CurrentTask;
            if (!task.IsValid())
            {
                Fail("Worker task is invalid.");
                return;
            }

            if (completedAmount >= task.Amount)
            {
                CompleteTask();
                return;
            }

            Machine machine = task.Workplace;
            if (!waitingForMachineBatch && machine.lavoro && machine.ricOra == task.Recipe)
            {
                expectedCompletedBatches = machine.CompletedBatches + 1;
                waitingForMachineBatch = true;
            }

            if (machine.lavoro && machine.ricOra != task.Recipe)
            {
                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
                continue;
            }

            if (waitingForMachineBatch)
            {
                if (machine.CompletedBatches >= expectedCompletedBatches)
                {
                    completedAmount++;
                    waitingForMachineBatch = false;
                    continue;
                }

                if (!machine.lavoro && !machine.TryStartRecipe(task.Recipe) && !machine.aspeEne)
                {
                    Fail("The machine could not start the assigned recipe.");
                    return;
                }

                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
                continue;
            }

            Dictionary<ResourceSO, int> missing = GetMissingMaterials(task.Recipe, machine);
            if (missing.Count > 0)
            {
                Dictionary<ResourceSO, int> carried = GetAvailableFromInventory(missing);
                if (carried.Count > 0)
                {
                    if (!AtWorkPoint(machine))
                    {
                        if (!MoveToWorkplace(machine))
                        {
                            Fail("Cannot pathfind to the machine.");
                            return;
                        }

                        await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
                        continue;
                    }

                    if (!machine.AddInputs(carried))
                    {
                        Fail("The machine refused its recipe inputs.");
                        return;
                    }

                    Inventory.RemoveItems(carried);

                    if (missing.All(item => carried.GetValueOrDefault(item.Key) >= item.Value) &&
                        !BeginMachineBatch(machine, task.Recipe))
                    {
                        Fail("The machine could not start the assigned recipe.");
                        return;
                    }

                    continue;
                }

                if (Inventory.CanAddItems(missing))
                {
                    if (Depot.Inventory == null || !Depot.Inventory.Contains(missing))
                    {
                        Fail("The depot does not have the required recipe inputs.");
                        return;
                    }

                    Depot source = FindDepotWithMaterials(missing);
                    if (source == null)
                    {
                        Fail("No depot is reachable for the required recipe inputs.");
                        return;
                    }

                    if (!AtWorkPoint(source))
                    {
                        if (!MoveToWorkplace(source))
                        {
                            Fail("Cannot pathfind to the depot.");
                            return;
                        }

                        await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
                        continue;
                    }

                    if (!Depot.Inventory.Contains(missing) || !Inventory.CanAddItems(missing))
                    {
                        await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
                        continue;
                    }

                    Depot.Inventory.RemoveItems(missing);
                    Inventory.AddItems(missing);
                    continue;
                }

                if (!machine.InputInventory.Contains(missing))
                {
                    if (Depot.Inventory == null || !Depot.Inventory.Contains(missing))
                    {
                        Fail("The depot does not have the required recipe inputs.");
                        return;
                    }

                    if (!machine.HasPendingInputDelivery && !machine.RequestInputsFromDepot(missing))
                    {
                        string reason = !machine.HasCarrierBots
                            ? "The worker cannot carry this recipe batch and no carrier is available."
                            : !machine.CanCarrierTransport(missing)
                                ? "No carrier can carry this recipe batch."
                                : "No reachable depot could schedule the required carrier delivery.";
                        Fail(reason);
                        return;
                    }

                    await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
                    continue;
                }
            }

            if (!AtWorkPoint(machine))
            {
                if (!MoveToWorkplace(machine))
                {
                    Fail("Cannot pathfind to the machine.");
                    return;
                }

                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
                continue;
            }

            if (!BeginMachineBatch(machine, task.Recipe))
            {
                Fail("The machine could not start the assigned recipe.");
                return;
            }

            await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
        }
    }

    Dictionary<ResourceSO, int> GetMissingMaterials(RecipeSO recipe, Machine machine)
    {
        Dictionary<ResourceSO, int> missing = new();
        foreach (var item in recipe.inputs)
        {
            int inMachine = machine.InputInventory.Items.GetValueOrDefault(item.Key);
            int amount = item.Value - inMachine;
            if (amount > 0)
                missing[item.Key] = amount;
        }

        return missing;
    }

    Dictionary<ResourceSO, int> GetAvailableFromInventory(Dictionary<ResourceSO, int> requested)
    {
        Dictionary<ResourceSO, int> available = new();
        foreach (var item in requested)
        {
            int amount = Mathf.Min(item.Value, Inventory.Items.GetValueOrDefault(item.Key));
            if (amount > 0)
                available[item.Key] = amount;
        }

        return available;
    }

    Depot FindDepotWithMaterials(Dictionary<ResourceSO, int> materials)
    {
        Depot closest = null;
        float closestDistance = float.PositiveInfinity;

        foreach (Depot depot in BuildingManager.GetBuildingsOfType<Depot>())
        {
            if (Depot.Inventory == null || !Depot.Inventory.Contains(materials) ||
                !LogisticsManager.TryGetAccessPoint(depot, Coords, out _))
                continue;

            float distance = (depot.Coords - Coords).sqrMagnitude;
            if (distance >= closestDistance)
                continue;

            closest = depot;
            closestDistance = distance;
        }

        return closest;
    }

    bool BeginMachineBatch(Machine machine, RecipeSO recipe)
    {
        if (waitingForMachineBatch)
            return true;

        if (machine.lavoro)
        {
            if (machine.ricOra != recipe)
                return false;
        }
        else
        {
            machine.SelectRecipe(recipe);
            if (!machine.TryStartRecipe(recipe) && !machine.aspeEne)
                return false;
        }

        expectedCompletedBatches = machine.CompletedBatches + 1;
        waitingForMachineBatch = true;
        return true;
    }

    void CompleteTask()
    {
        Machine machine = currentTask?.Workplace;
        currentTask = null;
        machine?.ReleaseWorker(this);
        completedAmount = 0;
        waitingForMachineBatch = false;
        expectedCompletedBatches = 0;
        destination = null;
        Path.Clear();
        StopWorking();
    }

    public override void Fail(string reason)
    {
        Debug.LogWarning(reason);
        CompleteTask();
    }

    public override void Dispose()
    {
        Machine machine = currentTask?.Workplace;
        currentTask = null;
        machine?.ReleaseWorker(this);
        completedAmount = 0;
        waitingForMachineBatch = false;
        expectedCompletedBatches = 0;
        base.Dispose();
    }
}
