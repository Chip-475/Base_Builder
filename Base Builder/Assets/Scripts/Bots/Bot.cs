//using Mono.Cecil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
    public string Name { get; protected set; }
    public float Power { get; protected set; }
    public int MaxWeight { get; protected set; }
    // ---
    public List<Vector3Int> Path { get; protected set; } = new();
    public Inventory Inventory { get; protected set; }
    public Bounds InteractionBounds { get; set; } = new();
    public bool PickedUp { get; protected set; }
    public bool Occupied { get; set; }
    public Machine WorkingAt { get; set; }
    public int MiningLevel { get; protected set; }

    [Header("IWorker")]
    public bool IsWorking { get; protected set; }
    public Coroutine WorkRoutine { get; protected set; }

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

        SceneObj.transform.position = Vector3.MoveTowards(Coords, Path[0], Data.speed * Time.deltaTime);
        if (Vector3.Distance(Coords, Path[0]) < 0.05f)
            Path.RemoveAt(0);
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

    public void StartWorking() => WorkRoutine = SceneObj.StartCoroutine(Pipeline());
    public virtual IEnumerator Pipeline()
    {
        yield break;
    }
    public void Fail(string reason)
    {
        Debug.Log(reason);
        SceneObj.StopCoroutine(WorkRoutine);
    }

    public Vector3 GetWorkPoint(Building workplace)
    {
        return workplace.SceneObj.BotWorkPoint != null
            ? workplace.SceneObj.BotWorkPoint.position
            : workplace.Coords;
    }
    public bool AtWorkPoint(Building workplace)
    {
        return (Coords - GetWorkPoint(workplace)).sqrMagnitude < 0.05f * 0.05f;
    }

    #region Interaction
    public void Interact()
    {
        if (!PickedUp)
        {
            SceneObj.transform.parent = PlayerManager.Instance.transform;
            SceneObj.transform.SetPositionAndRotation(PlayerManager.Instance.botCarryPoint.position, Quaternion.Euler(0, 0, 90));
            PickedUp = true;
            Occupied = false;
            WorkingAt = null;
            PlayerManager.Instance.IsCarryingBot = true;
        }
        else
        {
            SceneObj.transform.parent = null;
            SceneObj.transform.SetPositionAndRotation(PlayerManager.Instance.botDropPoint.position, Quaternion.Euler(0, 0, 0));
            PickedUp = false;
            PlayerManager.Instance.IsCarryingBot = false;
        }
    }
    public bool CanInteractWith()
    {
        throw new NotImplementedException();
    }
    #endregion
}
