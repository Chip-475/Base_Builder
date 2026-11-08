using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class Machine : Building, IInteractable, IWorkplace
{
    public new MachineData Data => base.Data as MachineData;
    public new MachineView SceneObj => base.SceneObj as MachineView;

    Vector3 IInteractable.Position => Coords;
    Cell[] IInteractable.InteractionCells => GetCellsInBounds(InteractionBounds);

    readonly Inventory inputInventory = new(int.MaxValue);
    readonly Inventory outputInventory = new(int.MaxValue);
    readonly Queue<Dictionary<ResourceSO, int>> pendingInputLoads = new();
    LogisticsManager.LogisticsRequest currentInputRequest;
    LogisticsManager.LogisticsRequest currentOutputRequest;
    CancellationTokenSource craftCancellationSource;
    bool disposed;

    public Inventory InputInventory => inputInventory;
    public Inventory OutputInventory => outputInventory;
    public Dictionary<ResourceSO, int> input => inputInventory.Items;
    public Dictionary<ResourceSO, int> output => outputInventory.Items;
    public bool Powered { get; private set; }
    public float RequestedPower { get; private set; }
    public MachineType Type { get; protected set; } = MachineType.None;
    public bool lavoro { get; private set; }
    public bool aspeEne { get; private set; }
    public RecipeSO ricOra { get; private set; }
    public RecipeSO SelectedRecipe { get; private set; }
    public float progresso { get; private set; }
    public int CompletedBatches { get; private set; }
    public WorkerBot AssignedWorker { get; private set; }
    public bool HasPendingInputDelivery => currentInputRequest != null || pendingInputLoads.Count > 0;
    public bool HasCarrierBots => BotManager.Bots.OfType<CarrierBot>().Any(bot => bot != null);

    public bool cuociAuto => Type == MachineType.Alloy_Furnace
        || Type == MachineType.Smelter
        || Type == MachineType.Furnace_Mk3;

    public Machine(MachineData data, MachineView sceneObj) : base(data, sceneObj)
    {
        Type = data.machineType;
        if (!PowerManager.machineDB.Contains(this))
            PowerManager.machineDB.Add(this);
        PowerManager.instance.RegisterBuilding();
        if (InteractionManager.Instance != null && !InteractionManager.Instance.interactables.Contains(this))
            InteractionManager.Instance.interactables.Add(this);
    }

    public override void Destroy()
    {
        Dispose();
        MonoBehaviour.Destroy(SceneObj.gameObject);
    }

    public void Dispose()
    {
        if (disposed)
            return;

        disposed = true;
        craftCancellationSource?.Cancel();
        StopPowerRequest();
        pendingInputLoads.Clear();

        if (AssignedWorker != null)
            AssignedWorker.CurrentTask = null;

        FreeUpCells(GetCellsInBounds(GetBounds()));
        BuildingManager.UnregisterBuilding(this);

        LogisticsManager.CancelRequest(currentInputRequest);
        LogisticsManager.CancelRequest(currentOutputRequest);

        currentInputRequest = null;
        currentOutputRequest = null;
        PowerManager.machineDB.Remove(this);
        if (InteractionManager.Instance != null)
            InteractionManager.Instance.interactables.Remove(this);
        if (PowerManager.instance != null)
            PowerManager.instance.UnregisterBuilding();
    }

    public bool RequestPower(float amount)
    {
        if (amount <= 0)
        {
            StopPowerRequest();
            return true;
        }

        NetworkManager currentNetwork = Network;
        if (currentNetwork == null || !currentNetwork.RequestPower(this, amount))
        {
            if (currentNetwork != null)
                currentNetwork.StopPowerRequest(this);
            RequestedPower = 0;
            Powered = false;
            return false;
        }

        RequestedPower = amount;
        Powered = true;
        return true;
    }

    public bool RequestPower(RecipeSO recipe)
    {
        return recipe != null && RequestPower(recipe.energyCost);
    }

    public void StopPowerRequest()
    {
        if (Network != null)
            Network.StopPowerRequest(this);
        RequestedPower = 0;
        Powered = false;
    }

    public void RefreshPowerRequest()
    {
        if (RequestedPower > 0)
            RequestPower(RequestedPower);
    }

    public bool CanInteractWith()
    {
        if (PlayerManager.Instance == null)
            return false;

        Cell playerCell = PlayerManager.Instance.GetPlayerCell();
        foreach (Cell cell in GetCellsInBounds(InteractionBounds))
            if (cell == playerCell)
                return true;

        return false;
    }

    public void Interact()
    {
        if (!CanInteractWith() || MachineUI.Instance == null)
            return;

        MachineUI.Instance.Open(this);
    }

    public bool CanAcceptInput(ResourceSO resource)
    {
        if (resource == null || Data == null || Data.usableRecipes == null)
            return false;

        return Data.usableRecipes.Any(recipe =>
            recipe != null && recipe.inputs != null && recipe.inputs.ContainsKey(resource));
    }

    public bool AddInputs(Dictionary<ResourceSO, int> resources)
    {
        if (resources == null || resources.Count == 0 ||
            resources.Any(item => !CanAcceptInput(item.Key) || item.Value <= 0) ||
            !inputInventory.CanAddItems(resources))
            return false;

        inputInventory.AddItems(resources);
        Tick();
        return true;
    }

    public bool aggIn(ResourceSO resource, int amount)
    {
        if (!CanAcceptInput(resource) || amount <= 0)
            return false;

        inputInventory.AddItem(resource, amount);
        Tick();
        return true;
    }

    public bool RequestInputsFromDepot(Dictionary<ResourceSO, int> resources)
    {
        if (resources == null || resources.Count == 0 || currentInputRequest != null ||
            pendingInputLoads.Count > 0 || Depot.Inventory == null || !Depot.Inventory.Contains(resources))
            return false;

        foreach (var item in resources)
            if (!CanAcceptInput(item.Key) || item.Value <= 0)
                return false;

        float capacity = GetLargestCarrierCapacity();
        List<Dictionary<ResourceSO, int>> loads = BuildLoads(resources, capacity);
        if (loads == null || loads.Count == 0)
            return false;

        foreach (Dictionary<ResourceSO, int> load in loads)
            pendingInputLoads.Enqueue(load);

        ScheduleNextInputLoad();
        return HasPendingInputDelivery;
    }

    public bool CanCarrierTransport(Dictionary<ResourceSO, int> resources)
    {
        return BuildLoads(resources, GetLargestCarrierCapacity()) != null;
    }

    public void RequestOutputDelivery()
    {
        ScheduleOutputDelivery();
    }

    public void SelectRecipe(RecipeSO recipe)
    {
        if (recipe == null || Data.usableRecipes == null || !Data.usableRecipes.Contains(recipe))
            return;

        SelectedRecipe = recipe;
        Tick();
    }

    public bool TryAssignWorker(WorkerBot worker, RecipeSO recipe)
    {
        if (worker == null || recipe == null || !CanUseWorker(worker) ||
            Data.usableRecipes == null || !Data.usableRecipes.Contains(recipe) ||
            (AssignedWorker != null && AssignedWorker != worker))
            return false;

        AssignedWorker = worker;
        Occupied = true;
        SelectedRecipe = recipe;
        return true;
    }

    public void ReleaseWorker(WorkerBot worker)
    {
        if (AssignedWorker != worker)
            return;

        AssignedWorker = null;
        Occupied = false;
        if (!lavoro)
            SelectedRecipe = null;
    }

    public bool CanUseWorker(WorkerBot worker)
    {
        bool specialist = worker != null && worker.Data != null && worker.Data.ID == "specialist";
        return worker != null && Data != null && (Data.needSpecialist == specialist);
    }

    public void RestoreState(
        Dictionary<ResourceSO, int> savedInputs,
        Dictionary<ResourceSO, int> savedOutputs,
        string selectedRecipeID,
        string inProgressRecipeID,
        float savedProgress,
        int completedBatches)
    {
        inputInventory.Items.Clear();
        outputInventory.Items.Clear();

        if (savedInputs != null)
            inputInventory.AddItems(savedInputs);
        if (savedOutputs != null)
            outputInventory.AddItems(savedOutputs);

        CompletedBatches = Mathf.Max(0, completedBatches);
        SelectedRecipe = Data.usableRecipes?.FirstOrDefault(recipe =>
            recipe != null && recipe.ID == selectedRecipeID);

        RecipeSO runningRecipe = Data.usableRecipes?.FirstOrDefault(recipe =>
            recipe != null && recipe.ID == inProgressRecipeID);
        if (runningRecipe == null || runningRecipe.inputs == null || runningRecipe.outputs == null)
        {
            Tick();
            return;
        }

        lavoro = true;
        ricOra = runningRecipe;
        SelectedRecipe = runningRecipe;
        progresso = Mathf.Clamp01(savedProgress);
        if (runningRecipe.energyCost > 0)
            RequestPower(runningRecipe);

        craftCancellationSource?.Dispose();
        craftCancellationSource = CancellationTokenSource.CreateLinkedTokenSource(
            SceneObj.GetCancellationTokenOnDestroy());
        Craft(runningRecipe, craftCancellationSource, progresso).Forget();
    }

    public void ClearSelectedRecipe(RecipeSO recipe)
    {
        if (SelectedRecipe == recipe && !lavoro)
            SelectedRecipe = null;
    }

    public bool start(RecipeSO recipe)
    {
        return CanStart(recipe);
    }

    public bool inputAll(RecipeSO recipe)
    {
        return recipe != null && recipe.inputs != null && recipe.inputs.Count > 0
            && inputInventory.Contains(recipe.inputs);
    }

    public bool TryStartRecipe(RecipeSO recipe)
    {
        if (!CanStart(recipe))
            return false;

        if (recipe.energyCost > 0 && !RequestPower(recipe))
        {
            aspeEne = true;
            return false;
        }

        inputInventory.RemoveItems(recipe.inputs);
        aspeEne = false;
        lavoro = true;
        ricOra = recipe;
        progresso = 0;
        craftCancellationSource?.Dispose();
        craftCancellationSource = CancellationTokenSource.CreateLinkedTokenSource(
            SceneObj.GetCancellationTokenOnDestroy());
        Craft(recipe, craftCancellationSource, 0).Forget();
        return true;
    }

    public void tryAutoStart()
    {
        if (disposed || lavoro || Data == null || Data.usableRecipes == null)
            return;

        RecipeSO recipe = SelectedRecipe;
        if (recipe != null)
        {
            TryStartRecipe(recipe);
            return;
        }

        if (!cuociAuto)
            return;

        foreach (RecipeSO candidate in Data.usableRecipes)
            if (CanStart(candidate))
            {
                TryStartRecipe(candidate);
                return;
            }

        aspeEne = false;
    }

    public void Tick()
    {
        if (disposed)
            return;

        if (lavoro && ricOra != null && ricOra.energyCost > 0 && !Powered)
            RequestPower(ricOra);
        else if (!lavoro)
            tryAutoStart();

        UpdateInputRequests();
        ScheduleOutputDelivery();

        if (cuociAuto && AssignedWorker == null && !lavoro &&
            currentInputRequest == null && pendingInputLoads.Count == 0)
            TryRequestAutomaticInputs();
    }

    bool CanStart(RecipeSO recipe)
    {
        return !disposed && !lavoro && recipe != null && recipe.inputs != null
            && recipe.outputs != null && Data != null && Data.usableRecipes != null
            && Data.usableRecipes.Contains(recipe) && inputAll(recipe);
    }

    async UniTask Craft(RecipeSO recipe, CancellationTokenSource source, float startingProgress)
    {
        CancellationToken token = source.Token;
        try
        {
            float duration = Mathf.Max(0, recipe.completionTime);
            float elapsed = duration * Mathf.Clamp01(startingProgress);
            progresso = duration <= 0 ? 1 : Mathf.Clamp01(elapsed / duration);
            while (elapsed < duration)
            {
                await UniTask.Yield(PlayerLoopTiming.Update, token);
                if (recipe.energyCost <= 0 || Powered)
                {
                    elapsed += Time.deltaTime;
                    progresso = Mathf.Clamp01(elapsed / duration);
                }
            }

            if (recipe.energyCost > 0)
                StopPowerRequest();

            outputInventory.AddItems(recipe.outputs);
            CompletedBatches++;
        }
        catch (OperationCanceledException) when (token.IsCancellationRequested)
        {
            // Destroying the machine cancels an in-progress batch.
        }
        catch (Exception exception)
        {
            Debug.LogException(exception, SceneObj);
        }
        finally
        {
            if (ReferenceEquals(craftCancellationSource, source))
            {
                craftCancellationSource = null;
                lavoro = false;
                ricOra = null;
                progresso = 0;
                if (AssignedWorker == null)
                    SelectedRecipe = null;
                source.Dispose();
            }
        }

        if (!disposed)
            Tick();
    }

    void TryRequestAutomaticInputs()
    {
        if (Data == null || Data.usableRecipes == null)
            return;

        foreach (RecipeSO recipe in Data.usableRecipes)
        {
            if (recipe == null || recipe.inputs == null || recipe.inputs.Count == 0 || Depot.Inventory == null)
                continue;

            Dictionary<ResourceSO, int> missing = new();
            foreach (var item in recipe.inputs)
            {
                int available = input.GetValueOrDefault(item.Key);
                int amount = item.Value - available;
                if (amount > 0)
                    missing[item.Key] = amount;
            }

            if (missing.Count == 0 || !Depot.Inventory.Contains(missing)
                || !HasCarrierBots || !CanCarrierTransport(missing))
                continue;

            RequestInputsFromDepot(missing);
            return;
        }
    }

    void UpdateInputRequests()
    {
        if (currentInputRequest != null && !LogisticsManager.Requests.Contains(currentInputRequest))
            currentInputRequest = null;

        if (currentInputRequest == null)
            ScheduleNextInputLoad();
    }

    void ScheduleNextInputLoad()
    {
        while (currentInputRequest == null && pendingInputLoads.Count > 0)
        {
            if (Depot.Inventory == null)
            {
                pendingInputLoads.Clear();
                return;
            }

            Dictionary<ResourceSO, int> load = pendingInputLoads.Dequeue();
            if (!Depot.Inventory.Contains(load))
            {
                pendingInputLoads.Clear();
                return;
            }

            Depot source = FindDepotForTransfer(load, Coords, requireSpace: false);
            if (source == null)
            {
                pendingInputLoads.Clear();
                return;
            }

            currentInputRequest = new LogisticsManager.LogisticsRequest(
                this, load, Depot.Inventory,
                source.SceneObj.BotWorkPoint != null ? source.SceneObj.BotWorkPoint.position : source.Coords,
                inputInventory, GetWorkPoint(), priority: 1);
            LogisticsManager.Requests.Add(currentInputRequest);
        }
    }

    void ScheduleOutputDelivery()
    {
        if (currentOutputRequest != null && !LogisticsManager.Requests.Contains(currentOutputRequest))
            currentOutputRequest = null;

        if (currentOutputRequest != null || outputInventory.IsEmpty || Depot.Inventory == null)
            return;

        float capacity = GetLargestCarrierCapacity();
        List<Dictionary<ResourceSO, int>> loads = BuildLoads(output, capacity);
        if (loads == null || loads.Count == 0)
            return;

        Dictionary<ResourceSO, int> load = loads[0];
        Depot destination = FindDepotForTransfer(load, Coords, requireSpace: true);
        if (destination == null)
            return;

        currentOutputRequest = new LogisticsManager.LogisticsRequest(
            this, load, outputInventory, GetWorkPoint(),
            Depot.Inventory,
            destination.SceneObj.BotWorkPoint != null ? destination.SceneObj.BotWorkPoint.position : destination.Coords,
            priority: 1);
        LogisticsManager.Requests.Add(currentOutputRequest);
    }

    Depot FindDepotForTransfer(Dictionary<ResourceSO, int> resources, Vector3 from, bool requireSpace)
    {
        Depot closest = null;
        float closestDistance = float.PositiveInfinity;

        foreach (Depot depot in BuildingManager.GetBuildingsOfType<Depot>())
        {
            if (requireSpace && !Depot.Inventory.CanAddItems(resources))
                continue;
            if (!LogisticsManager.TryGetAccessPoint(depot, from, out _))
                continue;

            float distance = (depot.Coords - from.ToVector3Int()).sqrMagnitude;
            if (distance >= closestDistance)
                continue;

            closest = depot;
            closestDistance = distance;
        }

        return closest;
    }

    float GetLargestCarrierCapacity()
    {
        return LogisticsManager.LargestCarrierCapacity;
    }

    static List<Dictionary<ResourceSO, int>> BuildLoads(
        Dictionary<ResourceSO, int> resources,
        float capacity)
    {
        return LogisticsManager.BuildLoads(resources, capacity);
    }

    Vector3 GetWorkPoint()
    {
        return SceneObj.BotWorkPoint != null
            ? SceneObj.BotWorkPoint.position
            : Coords;
    }

    public override Dictionary<ResourceSO, int> GetWorkOutputs()
    {
        return new Dictionary<ResourceSO, int>(output);
    }
}

public enum MachineType
{
    None = 0,
    Advanced_Assembler = 1,
    Alloy_Furnace = 2,
    Assembler = 3,
    Press = 4,
    Smelter = 5,
    Electric_Workbench = 6,
    Advanced_Press = 7,
    Advanced_Electric_Workbench = 8,
    Industrial_Assembler = 9,
    Industrial_Press = 10,
    Industrial_Electric_Workbench = 11,
    Furnace_Mk3 = 12,
}
