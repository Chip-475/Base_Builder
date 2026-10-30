using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

[Serializable]
public class Bot : IWorker, IInteractable
{
    public class BotConfig
    {
        public BotConfig() => Standardize();
        public static Transform baseSpawnPoint;

        public string id = "";
        public Vector3 baseSpawnCoords; // = baseSpawnPoint.position; 
        public string name = "";
        public const float maxPower = 100;
        public float power = 0;
        public int maxWeight = 15;

        public void Standardize()
        {
            if (string.IsNullOrEmpty(id))
                id = new GUID().ToString();

            // coords

            if (string.IsNullOrEmpty(name))
                name = PickRandomName();

            if (power == 0)
                power = 20;
        }

        public string PickRandomName()
        {
            string[] names_1 = { "Alpha", "Bravo", "Charlie", "Delta", "Echo", "Foxtrot", "Golf", "Hotel", "India", "Juliett" };
            string[] names_2 = { "Leader", "Keeper", "Pioneer", "Witcher", "Diver", "Bomber", "Rancher", "Taker", "Dispatcher", "Trickster" };
            return names_1[UnityEngine.Random.Range(0, names_1.Length)] + " " + names_2[UnityEngine.Random.Range(0, names_2.Length)];
        }
    }

    public BotData Data { get; protected set; }
    public BotView SceneObj { get; protected set; }

    [Header("Runtime Data")]
    public string Id { get; protected set; }
    public Vector3 Coords
    {
        get => SceneObj.transform.position;
        set => SceneObj.transform.position = value;
    }
    public Cell CurrentCell {get => WorldManager.World.GetCellAt(Coords.ToVector3Int());}
    public string Name { get; protected set; }
    public float localSpeed;
    float power;
    public float Power { get => power; protected set { power = value; if(power <= 0) IsOn = false; else IsOn=true; }  }
     bool isOn;
    public bool IsOn
    {
        get => isOn;
        set
        {
            isOn = value;
        if (value) TurnOn();
            else ShutDown();
        }
    }
    public float PowerConsumption
    {
        get => Data.powerConsumption; // * powerEfficiency
    }
    public int MaxWeight { get; protected set; }
    // ---
    public List<Vector3Int> Path { get; protected set; } = new();
    public Inventory Inventory { get; protected set; }
    public Bounds InteractionBounds { get; set; } = new();
    public bool PickedUp { get; protected set; }
    public Machine WorkingAt { get; set; }
    public int MiningLevel { get; protected set; }

    [Header("IWorker")]
    public bool Occupied { get; protected set; }
    public UniTask WorkRoutine { get; protected set; }
    CancellationTokenSource workCancellationSource;

    [Header("IInteractable")]
    public Vector3 Position => SceneObj.transform.position;
    public Cell[] InteractionCells => Helpers.GetCellsInBounds(InteractionBounds);

    // Runtime
    protected Vector3Int? destination;


    public Bot(BotView view, BotData data, BotConfig config)
    {
        SceneObj = view;
        Data = data;

        // Config
        Id = config.id;
        Name = config.name;
        Power = config.power;
        MaxWeight = data.carryCapacity;
        // ---
        Inventory = new Inventory(data.carryCapacity);
        SceneObj.name = Name;

        // Initiation
        BotManager.RegisterBot(this);
        InteractionManager.Instance.interactables.Add(this);
        view.StartCoroutine(Recharge());
    }
    public void ShutDown()
    {
        localSpeed=0;
        StopWorking();
    }
    public void TurnOn()
    {
        localSpeed=Data.speed;
        //should stop working but remember current workspace
    }
    #region Movement
    public void SetDestination(Vector3Int coords)
    {
        var myPos = Coords.ToVector3Int();
        Pathfinder.Pathfind(myPos, coords, out List<Vector3Int> path);
        Path = new(path);
    }
    public void MoveTowardsDestination()
    {
        if (Path.Count <= 0)
            return;

        power -= Data.powerConsumption;
        Vector3 previousCoords = Position;

        SceneObj.transform.position = Vector3.MoveTowards(Coords, Path[0], localSpeed * Time.deltaTime);
        if (Position.x - previousCoords.x != 0)
            SceneObj.spriteRenderer.flipX = Mathf.Sign(Position.x - previousCoords.x) < 0;

        if (Vector3.Distance(Coords, Path[0]) < 0.05f)
            Path.RemoveAt(0);
    }
    IEnumerator Recharge()
    {
        while (true)
        {
            if (CurrentCell.isPowered)
            Power += 1;

            yield return null;
        }  
    }

    public bool MoveTo(Vector3Int target)
    {
        if (destination != target || Path.Count == 0)
        {
            if (!Pathfinder.CanPathfind(Coords.ToVector3Int(), target))
                return false;

            SetDestination(target);
            destination = target;
        }

        MoveTowardsDestination();
        return true;
    }
    public bool MoveToWorkplace(Building workplace)
    {
        
        Vector3 workPoint = workplace.SceneObj.BotWorkPoint != null
            ? workplace.SceneObj.BotWorkPoint.position
            : workplace.Coords;
        Vector3Int target = workPoint.ToVector3Int();

        if (destination != target || Path.Count == 0)
        {
            if (!Pathfinder.CanPathfind(Coords.ToVector3Int(), target))
                return false;

            SetDestination(target);
            destination = target;
        }

        MoveTowardsDestination();
        return true;
    }

    #endregion

    // Work
    public Bot GetBot() { return this; }

    public void StartWorking()
    {
        if (Occupied)
            return;

        workCancellationSource?.Dispose();
        workCancellationSource = CancellationTokenSource.CreateLinkedTokenSource(
            SceneObj.GetCancellationTokenOnDestroy());
        Occupied = true;
        WorkRoutine = RunPipeline(workCancellationSource.Token);
        WorkRoutine.Forget();
    }

    async UniTask RunPipeline(CancellationToken cancellationToken)
    {
        try
        {
            await Pipeline(cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            // Cancellation is the normal way for a bot to stop working.
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
        }
        finally
        {
            Occupied = false;
            workCancellationSource?.Dispose();
            workCancellationSource = null;
        }
    }

    public virtual UniTask Pipeline(CancellationToken cancellationToken)
    {
        return UniTask.CompletedTask;
    }

    public void StopWorking()
    {
        workCancellationSource?.Cancel();
        Occupied = false;
    }

    public virtual void Fail(string reason)
    {
        Debug.Log(reason);
        StopWorking();
    }

    public Vector3 GetWorkPoint(Building workplace)
    {
        return workplace.SceneObj.BotWorkPoint != null
            ? workplace.SceneObj.BotWorkPoint.position
            : workplace.Coords;
    }
    public bool AtWorkPoint(Building workplace)
    {
        return (Coords - GetWorkPoint(workplace)).sqrMagnitude < 1f * 1f;
    }

    #region Interaction
    public void Interact()
    {
        if (!PickedUp)
        {
            SceneObj.transform.parent = PlayerManager.Instance.transform;
            SceneObj.transform.SetPositionAndRotation(PlayerManager.Instance.botCarryPoint.position, Quaternion.Euler(0, 0, 90));
            PickedUp = true;
            Occupied = true;
            WorkingAt = null;
            StopWorking();
            PlayerManager.Instance.IsCarryingBot = true;
        }
        else
        {
            SceneObj.transform.parent = null;
            SceneObj.transform.SetPositionAndRotation(PlayerManager.Instance.botDropPoint.position, Quaternion.Euler(0, 0, 0));
            PickedUp = false;
            Occupied = false;
            PlayerManager.Instance.IsCarryingBot = false;
        }
    }
    public bool CanInteractWith()
    {
        throw new NotImplementedException();
    }
    #endregion
}
