using UnityEngine;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[Serializable]
public abstract class Bot : IWorker, IInteractable
{
    public class BotConfig
    {
        public BotConfig() => Standardize();
        public static Transform baseSpawnPoint;

        public string id = "";
        public Vector3 coords = baseSpawnPoint.position;
        public string name = "";
        public const float maxPower = 100;
        public float power = 0;
        public int maxWeight = 15;

        public void Standardize()
        {
            if(string.IsNullOrEmpty(id))
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
    /// <summary>
    /// Always call Standardize() when creating.
    /// </summary>
    public struct WorkTask
    {
        public IWorker Worker { get; set; }
        public IWorkplace Workplace { get; set; }
        public RecipeSO Recipe { get; set; }
        public Dictionary<ResourceSO, int> Inputs { get; set; }
        public Dictionary<ResourceSO, int> Output { get; set; }

        public void Standardize()
        {
            Inputs = new(Recipe != null ? Recipe.inputs : null);
            Output = new(Recipe != null ? Recipe.outputs : null);
        }
    }

    public BotView BotView { get; protected set; }
    public BotData BotData { get; protected set; }

    [Header("Runtime Data")]
    public string Id { get; protected set; }
    public Vector3 Coords { get; protected set; }
    public string Name { get; protected set; }
    public float Power { get; protected set; }
    public int MaxWeight { get; protected set; }
    // ---
    public BotInventory Inventory { get; protected set; }
    public Bounds InteractionBounds { get; set; } = new();
    public bool PickedUp { get; protected set; }
    public bool Occupied { get; set; }
    public Machine WorkingAt { get; set; }
    public int MiningLevel { get; protected set; }

    [Header("Interfaces")]
    public Vector3 Position => BotView.transform.position;
    public Cell[] InteractionCells => Helpers.GetCellsInBounds(InteractionBounds);

    public Bot(BotView view, BotData data, BotConfig config)
    {
        BotView = view;
        BotData = data;

        // Config
        Id = config.id;
        Coords = config.coords;
        Name = config.name;
        Power = config.power;
        MaxWeight = data.carryCapacity;
        // ---
        Inventory = new BotInventory(data.carryCapacity);
        BotView.name = Name;

        // Initiation
        BotManager.RegisterBot(this);
        InteractionManager.Instance.interactables.Add(this);
    }

    // IWorker
    public Bot GetBot() { return this; }
    public abstract bool CanPerform(WorkTask task);
    public abstract void Perform(WorkTask task);
    public abstract bool Fail(WorkTask task, string reason);

    // IInteractable
    public void Interact()
    {
        if (!PickedUp)
        {
            BotView.transform.parent = PlayerManager.Instance.transform;
            BotView.transform.SetPositionAndRotation(PlayerManager.Instance.botCarryPoint.position, Quaternion.Euler(0, 0, 90));
            PickedUp = true;
            Occupied = false;
            WorkingAt = null;
            PlayerManager.Instance.IsCarryingBot = true;
        }
        else
        {
            BotView.transform.parent = null;
            BotView.transform.SetPositionAndRotation(PlayerManager.Instance.botDropPoint.position, Quaternion.Euler(0, 0, 0));
            PickedUp = false;
            PlayerManager.Instance.IsCarryingBot = false;
        }
    }
    public bool CanInteractWith()
    {
        throw new NotImplementedException();
    }

    public class BotInventory
    {
        public Dictionary<ResourceSO, int> Items { get; private set; } = new();
        public int MaxWeight { get; private set; }

        public BotInventory(BotInventory inv)
        {
            Items = new(inv.Items);
            MaxWeight = inv.MaxWeight;
        }
        public BotInventory(int maxWeight)
        {
            MaxWeight = maxWeight;
        }

        public void AddItem(ResourceSO resource, int amount)
        {
            if(!CanAddItem(resource, amount))
            {
                Debug.Log("Cannot add item.");
                return;
            }

            Items[resource] = Items.GetValueOrDefault(resource) + amount;
        }
        public bool CanAddItem(ResourceSO resource, int amount)
        {
            float requestWeight = resource.weightPerUnit * amount;
            float newWeight = requestWeight + GetWeight();
            if (newWeight > MaxWeight)
                return false;

            return true;
        }

        public KeyValuePair<ResourceSO, int> RemoveItem(ResourceSO resource, int amount)
        {
            if(!CanRemoveItem(resource, amount))
            {
                Debug.Log("Cannot remove item.");
                return new(null, 0);
            }

            Items[resource] = Items.GetValueOrDefault(resource) - amount;
            return new(resource, amount);
        }
        public bool CanRemoveItem(ResourceSO resource, int amount)
        {
            if (!Items.ContainsKey(resource))
                return false;
            if (Items[resource] - amount < 0)
                return false;

            return true;
        }

        public float GetWeight()
        {
            float totWeight = 0f;
            foreach(var kvp in Items)
                totWeight += kvp.Key.weightPerUnit * kvp.Value;

            return totWeight;
        }
        public float GetSpecificWeight(ResourceSO resource)
        {
            float weight = 0f;
            foreach (var kvp in Items)
                if(kvp.Key == resource) 
                    weight += kvp.Key.weightPerUnit * kvp.Value;

            return weight;
        }
    }
}
