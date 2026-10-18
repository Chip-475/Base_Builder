using UnityEngine;
using System;
using System.Collections.Generic;

public enum BotType
{
    None,
    Miner,
    Carrier,
    Worker,
    Builder,
    Specialist
}
[Serializable]
public class Bot : IInteractable
{
    public BotView BotView { get; protected set; }
    public BotData BotData { get; protected set; }
    public BotType Type { get; protected set; } = BotType.None;

    [Header("Runtime Data")]
    public string Id { get; protected set; }
    public Vector3 Coords { get; protected set; }
    public string Name { get; protected set; }
    public float Power { get; protected set; }
    public int CarryCapacity { get; protected set; }
    public BotInventory Inventory { get; protected set; }
    public Bounds InteractionBounds { get; set; } = new();
    public bool PickedUp { get; protected set; }
    public bool Occupied { get; set; }
    public Machine WorkingAt { get; set; }
    public int MiningLevel { get; protected set; }

    [Header("Intefaces")]
    public Vector3 Position => BotView.transform.position;
    public Cell[] InteractionCells => Helpers.GetCellsInBounds(InteractionBounds);

    public Bot(BotView view, BotData data, string id = null, Vector3? coords = null, string name = null, float power = 100)
    {
        BotView = view;
        BotData = data;
        Type = data.type;

        // Self
        Id = id ?? Guid.NewGuid().ToString();
        Coords = coords ?? Vector3.zero;
        Name = name ?? PickRandomName();
        Power = power;
        CarryCapacity = data.carryCapacity;
        Inventory = new BotInventory(data.carryCapacity);
        
        // View
        BotView.name = Name;

        // ID
        BotManager.RegisterBot(this);
        InteractionManager.Instance.interactables.Add(this);
    }

    string PickRandomName()
    {
        string[] names_1 = { "Alpha", "Bravo", "Charlie", "Delta", "Echo", "Foxtrot", "Golf", "Hotel", "India", "Juliett" };
        string[] names_2 = { "Leader", "Keeper", "Pioneer", "Witcher", "Diver", "Bomber", "Rancher", "Taker", "Dispatcher", "Trickster" };
        return names_1[UnityEngine.Random.Range(0, names_1.Length)] + " " + names_2[UnityEngine.Random.Range(0, names_2.Length)];
    }

    public bool CanInteractWith()
    {
        throw new NotImplementedException();
    }
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
