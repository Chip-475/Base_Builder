using UnityEngine;
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

[Serializable]
public class Bot
{
    public BotView BotView { get; protected set; }
    public BotData BotData { get; protected set; }
    public BotType Type { get; protected set; } = BotType.None;
    public BuildingData Bd {get; protected set; }
    public Building ToBuild {get; protected set; }

    [Header("Runtime Data")]
    public string Id { get; protected set; }
    public Vector3 Coords {  get; protected set; }
    public string Name { get; protected set; }
    public float Power { get; protected set; }
    public int CarryCapacity { get; protected set; }
    public BotInventory Inventory { get; protected set; }

    public RecipeSO recipeOra { get; set; }
    public ResourceSO inputRisorse {  get; set; }

    public Bot(BotView view, BotData data, string id = null, Vector3? coords = null, string name = null, float power = 100)
    {
        //realizzare che i bot si possano prendere e portare in giro
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
    }

    public bool MoveTo(Vector3Int coords)
    {
        /*
        BotView.SetDestination(coords);
        BotView.StartMoving();*/
        bool raggiungibile = BotView.SetDestination(coords);

        if (raggiungibile)
        {
            BotView.StartMoving();
        }

        return raggiungibile;
    }
    async UniTask<bool> muoviAsp(Vector3Int coords)
    {
        if (!MoveTo(coords))
        {
            Debug.Log(Name + ": nessun percorso verso " + coords);
            return false;
        }

        await UniTask.WaitUntil(() => !BotView.IsMoving, cancellationToken: BotView.GetCancellationTokenOnDestroy());
        return true;
    }

    public async UniTask goToMineralNode(MineralNode mineralNode)
    {
        if (Type != BotType.Miner)return;
        bool arrivato = await muoviAsp(mineralNode.SceneObj.transform.position.ToVector3Int());
        if (arrivato)mineralNode.MineResource();
    }
    string PickRandomName()
    {
        string[] names_1 = { "Alpha", "Bravo", "Charlie", "Delta", "Echo", "Foxtrot", "Golf", "Hotel", "India", "Juliett" };
        string[] names_2 = { "Leader", "Keeper", "Pioneer", "Witcher", "Diver", "Bomber", "Rancher", "Taker", "Dispatcher", "Trickster" };
        return names_1[UnityEngine.Random.Range(0, names_1.Length)] + " " + names_2[UnityEngine.Random.Range(0, names_2.Length)];
    }
    /*
    public async UniTask goToMineralNode(MineralNode mineralNode)
    {
        if(Type == BotType.Miner)
        {
            MoveTo(mineralNode.SceneObj.transform.position.ToVector3Int());
            await UniTask.WaitUntil(() => Vector3.Distance(BotView.transform.position, mineralNode.SceneObj.transform.position) < 0.1f);
            mineralNode.MineResource();
        }
    }
    */
    public async UniTask goToSmelter(Machine machine)
    {
        if(Type == BotType.Worker)
        {
            MoveTo(machine.SceneObj.transform.position.ToVector3Int());
            await UniTask.WaitUntil(() => Vector3.Distance(BotView.transform.position, machine.SceneObj.transform.position) < 0.1f);
            await machine.UseSmelter(BotData.currentRecipe, BotData.currentInputResources);
        }
    }
    public async UniTask goToAssembler(Machine machine)
    {
        if(Type == BotType.Worker)
        {
            MoveTo(machine.SceneObj.transform.position.ToVector3Int());
            await UniTask.WaitUntil(() => Vector3.Distance(BotView.transform.position, machine.SceneObj.transform.position) < 0.1f);
            await machine.UseAssembler(BotData.currentRecipe, BotData.currentInputResources);
        }
    }
    public async UniTask goToPress(Machine machine)
    {
        if(Type == BotType.Worker)
        {
            MoveTo(machine.SceneObj.transform.position.ToVector3Int());
            await UniTask.WaitUntil(() => Vector3.Distance(BotView.transform.position, machine.SceneObj.transform.position) < 0.1f);
            await machine.UsePress(BotData.currentRecipe, BotData.currentInputResources);
        }
    }
    public async UniTask goToAlloyFurnace(Machine machine)
    {
        if(Type == BotType.Worker)
        {
            MoveTo(machine.SceneObj.transform.position.ToVector3Int());
            await UniTask.WaitUntil(() => Vector3.Distance(BotView.transform.position, machine.SceneObj.transform.position) < 0.1f);
            await machine.UseAlloyFurnace(BotData.currentRecipe, BotData.currentInputResources);
        }
    }
    public async UniTask goToAdvancedAssembler(Machine machine)
    {
        if(Type == BotType.Worker)
        {
            MoveTo(machine.SceneObj.transform.position.ToVector3Int());
            await UniTask.WaitUntil(() => Vector3.Distance(BotView.transform.position, machine.SceneObj.transform.position) < 0.1f);
            await machine.UseAdvancedAssembler(BotData.currentRecipe, BotData.currentInputResources);
        }
}
        public async UniTask Build(Building build)
    {
        if(Type == BotType.Builder)
        {
            MoveTo(build.SceneObj.transform.position.ToVector3Int());
            await UniTask.WaitUntil(() => Vector3.Distance(BotView.transform.position,build.SceneObj.transform.position)< 0.1f);
            await UniTask.WaitForSeconds(Bd.timeToBuild);
            Bd.builded=true;
            return;
        }
        Bd.builded=false;
        return;
    }
    public async UniTask goToBuilderHouse(Building BuilderHouse)
    {
        if(Type == BotType.Builder)
        {
            MoveTo(BuilderHouse.SceneObj.transform.position.ToVector3Int());
            await UniTask.WaitUntil(() => Vector3.Distance(BotView.transform.position,BuilderHouse.SceneObj.transform.position)< 0.1f);


        }
    }
        //fare sistema che lui giri intondo all oggetto mentre lo builda o se disoccupato intorno casa sua 3x3
        //bot builder va fatto che passeggia in giro o che stia fermo quando non builda e va preso e andato 
        //a mettere a buildare lui costruisce per il tempo necessario e dopodiche si ferma o torna a casa
}
public class BotInventory
{
    public Dictionary<ResourceSO, int> Inventory { get; private set; } = new();
    public int MaxWeight { get; private set; }

    public BotInventory(BotInventory inv)
    {
        Inventory = new(inv.Inventory);
        MaxWeight = inv.MaxWeight;
    }
    public BotInventory(int maxWeight)
    {
        MaxWeight = maxWeight;
    }
   
}


public enum BotType
{
    None,
    Miner,
    Carrier,
    Worker,
    Builder,
    Specialist
}
