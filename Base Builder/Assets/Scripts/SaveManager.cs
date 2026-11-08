using Newtonsoft.Json;
using System.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

// vitto te lo giuro se tocchi sta classe ti fucilo
public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    [SerializeField] Button saveButton; // Only for testing
    [SerializeField] Button loadButton; // Only for testing

    string savePath;
    public List<BuildingView> buildingPrefabs = new();
    public List<BotView> botPrefabs = new();
    public List<ResourceSO> resourceDB = new();

    public SaveFile file = new();

    private void Awake()
    {
        Instance = this;
        TechTree.Initialize();

        savePath = Path.Combine(Application.persistentDataPath, "save.json");

        saveButton.onClick.AddListener(() => file.SaveAll(savePath));
        loadButton.onClick.AddListener(() => file.LoadFrom(savePath));

        #if !UNITY_EDITOR
            saveButton.gameObject.SetActive(false);
        #endif
    }
    private void Start()
    {
        file.LoadFrom(savePath);
    }
}

// anche questa
[Serializable]
public class SaveFile
{
    // To Save
    public PlayerSaveData playerData;
    public List<BuildingSaveData> buildingsData = new();
    public List<ConstructionSiteSaveData> constructionSitesData = new();
    public List<BotSaveData> botsData = new();
    public DepotSaveData depotData;
    public List<TechSaveData> techsData = new();
    // - Settings

    public void SaveAll(string path)
    {
        if (PlayerManager.Instance == null)
        {
            Debug.LogError("Cannot save: PlayerManager has not been initialized.");
            return;
        }

        if (Depot.Inventory == null)
        {
            Debug.LogError("Cannot save: Depot has not been initialized.");
            return;
        }

        buildingsData.Clear();
        constructionSitesData.Clear();
        botsData.Clear();
        techsData.Clear();

        playerData = new(PlayerManager.Instance);

        foreach (var building in BuildingManager.Buildings.Values.Where(b => b is not MineralNode))
            buildingsData.Add(new(building));

        foreach (var site in ConstructionSite.Sites)
            if (site != null && site.IsPending)
                constructionSitesData.Add(new(site));

        foreach (var bot in BotManager.Bots)
            botsData.Add(new(bot));

        depotData = new(Depot.Inventory);
        
        foreach (var tech in TechTree.TechDB.Values)
            techsData.Add(new(tech));

        // settings

        WriteTo(path);
    }
    public void LoadAll()
    {
        playerData?.Load();

        foreach (var data in buildingsData)
            data?.Load();

        foreach (var data in constructionSitesData)
            data?.Load();

        // Instantiated objects need one frame to run their Start methods and
        // register themselves before bots and their tasks can be restored.
        if (SaveManager.Instance != null)
            SaveManager.Instance.StartCoroutine(LoadRuntimeData());
    }

    IEnumerator LoadRuntimeData()
    {
        yield return null;

        depotData?.Load();

        foreach (var data in techsData)
            data?.Load();

        foreach (var data in botsData)
            yield return data.Load();

        // Buildings and bots are now registered, so task targets can be found.
        foreach (var data in botsData)
            data.RestoreTask();
    }
    
    void WriteTo(string path)
    {
        string json = JsonConvert.SerializeObject(this, Formatting.Indented);
        File.WriteAllText(path, json);
    }
    public void LoadFrom(string path)
    {
        if (!File.Exists(path))
        {
            Debug.LogWarning($"Save file not found: {path}");
            return;
        }

        string json = File.ReadAllText(path);
        SaveFile loadedFile = JsonConvert.DeserializeObject<SaveFile>(json);

        if (loadedFile == null)
        {
            Debug.LogError($"Could not deserialize save file: {path}");
            return;
        }

        playerData = loadedFile.playerData;
        buildingsData = loadedFile.buildingsData ?? new();
        constructionSitesData = loadedFile.constructionSitesData ?? new();
        botsData = loadedFile.botsData ?? new();
        depotData = loadedFile.depotData;
        techsData = loadedFile.techsData ?? new();

        LoadAll();
    }
}

[Serializable]
public class PlayerSaveData
{
    public Helpers.SerializableVector3 position;

    // Required by Unity and Newtonsoft.Json. Do not access scene objects here:
    // this constructor can run before PlayerManager.Awake().
    public PlayerSaveData() { }

    public PlayerSaveData(PlayerManager player)
    {
        position = new(player.transform.position);
    }
    public void Load()
    {
        PlayerManager.Instance.gameObject.transform.position = position.ToVector3();
    }
}

[Serializable]
public class BuildingSaveData
{
    public string buildingID;
    public Helpers.SerializableVector3 position;

    // Required by Newtonsoft.Json when loading a saved BuildingSaveData.
    public BuildingSaveData() { }

    public BuildingSaveData(Building building)
    {
        buildingID = building.Data.ID;
        position = new(building.Coords);
    }
    public void Load()
    {
        BuildingView prefab = SaveManager.Instance.buildingPrefabs.FirstOrDefault(prefab => prefab.Data.ID == buildingID);

        if (prefab != null)
            MonoBehaviour.Instantiate(prefab, position.ToVector3(), Quaternion.identity);
    }
}

[Serializable]
public class ConstructionSiteSaveData
{
    public string buildingID;
    public Helpers.SerializableVector3 position;

    // Required by Newtonsoft.Json when loading a construction site.
    public ConstructionSiteSaveData() { }

    public ConstructionSiteSaveData(ConstructionSite site)
    {
        buildingID = site.Prefab.Data.ID;
        position = new(site.transform.position);
    }

    public void Load()
    {
        BuildingView prefab = SaveManager.Instance.buildingPrefabs
            .FirstOrDefault(prefab => prefab != null && prefab.Data != null && prefab.Data.ID == buildingID);

        if (prefab != null)
            ConstructionSite.Create(prefab, position.ToVector3());
        else
            Debug.LogWarning($"Building prefab not found for construction site '{buildingID}'.");
    }
}

[Serializable]
public class BotSaveData
{
    public string botID;
    public string runtimeID;
    public string botName;
    public Helpers.SerializableVector3 position;
    public float power;
    public List<InventoryItemSaveData> inventory = new();
    public BotTaskSaveData task;

    [NonSerialized] Bot runtimeBot;

    // Required by Newtonsoft.Json when loading a BotSaveData.
    public BotSaveData() { }

    public BotSaveData(Bot bot)
    {
        botID = bot.Data.ID;
        runtimeID = bot.Id;
        botName = bot.Name;
        position = new(bot.Coords);
        power = bot.Power;

        foreach (var item in bot.Inventory.Items)
        {
            if (item.Key == null)
                continue;

            inventory.Add(new InventoryItemSaveData
            {
                resourceID = item.Key.ID,
                amount = item.Value
            });
        }

        if (bot is WorkerBot worker && worker.CurrentTask != null)
        {
            task = new BotTaskSaveData
            {
                type = "Worker",
                recipeID = worker.CurrentTask.Recipe != null
                    ? worker.CurrentTask.Recipe.ID
                    : string.Empty,
                amount = worker.CurrentTask.Amount,
                workplacePosition = worker.CurrentTask.Workplace != null
                    ? new Helpers.SerializableVector3(worker.CurrentTask.Workplace.Coords)
                    : default
            };
        }
        else if (bot is MinerBot miner && miner.Node != null && miner.Crate != null)
        {
            task = new BotTaskSaveData
            {
                type = "Miner",
                nodePosition = new Helpers.SerializableVector3(miner.Node.Coords),
                cratePosition = new Helpers.SerializableVector3(miner.Crate.Coords)
            };
        }
        else if (bot is CarrierBot carrier && carrier.request != null)
        {
            // The request itself is intentionally not serialized because it
            // contains live Inventory references. The carrier will claim a
            // fresh request after it is loaded.
            task = new BotTaskSaveData { type = "Carrier" };
        }
    }

    public IEnumerator Load()
    {
        BotView prefab = SaveManager.Instance.botPrefabs
            .FirstOrDefault(p => p != null && p.data != null && p.data.ID == botID);

        if (prefab == null)
        {
            Debug.LogWarning($"Bot prefab not found for data ID '{botID}'.");
            yield break;
        }

        BotView view = MonoBehaviour.Instantiate(prefab, position.ToVector3(), Quaternion.identity);

        // BotView creates its runtime Bot in Start().
        yield return null;

        runtimeBot = view.RuntimeObj;
        if (runtimeBot == null)
        {
            Debug.LogWarning($"Bot runtime object was not initialized for '{botID}'.");
            yield break;
        }

        runtimeBot.RestoreIdentity(runtimeID, botName);
        runtimeBot.Power = power;
        runtimeBot.Inventory.Items.Clear();

        foreach (var item in inventory)
        {
            ResourceSO resource = SaveManager.Instance.resourceDB
                .FirstOrDefault(resource => resource != null && resource.ID == item.resourceID);

            if (resource == null)
            {
                Debug.LogWarning($"Resource not found for saved bot inventory item '{item.resourceID}'.");
                continue;
            }

            runtimeBot.Inventory.Items[resource] = item.amount;
        }
    }

    public void RestoreTask()
    {
        if (runtimeBot == null || task == null)
            return;

        if (task.type == "Worker" && runtimeBot is WorkerBot worker)
        {
            Machine workplace = BuildingManager.Buildings.Values
                .OfType<Machine>()
                .FirstOrDefault(machine =>
                    machine.Coords == task.workplacePosition.ToVector3().ToVector3Int());

            if (workplace == null)
            {
                Debug.LogWarning("Could not restore worker task: workplace not found.");
                return;
            }

            RecipeSO recipe = workplace.Data.usableRecipes
                .FirstOrDefault(recipe => recipe != null && recipe.ID == task.recipeID);

            if (recipe == null)
            {
                Debug.LogWarning($"Could not restore worker task: recipe '{task.recipeID}' not found.");
                return;
            }

            worker.CurrentTask = new WorkerBot.Task(worker, workplace, recipe, task.amount);
        }
        else if (task.type == "Miner" && runtimeBot is MinerBot miner)
        {
            miner.Node = BuildingManager.Buildings.Values
                .OfType<MineralNode>()
                .FirstOrDefault(node =>
                    node.Coords == task.nodePosition.ToVector3().ToVector3Int());

            miner.Crate = BuildingManager.Buildings.Values
                .OfType<MineralCrate>()
                .FirstOrDefault(crate =>
                    crate.Coords == task.cratePosition.ToVector3().ToVector3Int());

            if (miner.Node == null || miner.Crate == null)
            {
                Debug.LogWarning("Could not restore miner task: node or crate not found.");
                return;
            }

            miner.StartWorking();
        }
    }
}

[Serializable]
public class BotTaskSaveData
{
    public string type;
    public string recipeID;
    public int amount;
    public Helpers.SerializableVector3 workplacePosition;
    public Helpers.SerializableVector3 nodePosition;
    public Helpers.SerializableVector3 cratePosition;
}

[Serializable]
public class InventoryItemSaveData
{
    public string resourceID;
    public int amount;
}

[Serializable]
public class DepotSaveData
{
    [SerializeField] public Dictionary<string, int> items = new();

    // Required by Unity and Newtonsoft.Json. Do not access the scene inventory
    // here because this constructor can run before the depot is initialized.
    public DepotSaveData() { }

    public DepotSaveData(Inventory inventory)
    {
        foreach (var item in inventory.Items)
            items.Add(item.Key.ID, item.Value);
    }
    public void Load()
    {
        if (Depot.Inventory == null)
        {
            Debug.LogWarning("Could not load depot inventory: no depot has been initialized.");
            return;
        }

        Depot.Inventory.Items.Clear();

        foreach (var item in items)
        {
            ResourceSO resource = SaveManager.Instance.resourceDB
                .FirstOrDefault(res => res != null && res.ID == item.Key);

            if (resource == null)
            {
                Debug.LogWarning($"Resource not found for saved depot item '{item.Key}'.");
                continue;
            }

            Depot.Inventory.Items[resource] = item.Value;
        }
    }
}

[Serializable]
public class TechSaveData
{
    public string techID;
    public bool unlocked;

    // Required by Unity and Newtonsoft.Json when loading saved tech data.
    public TechSaveData() { }

    public TechSaveData(TechSO tech)
    {
        techID = tech.ID;
        unlocked = TechTree.UnlockedTechs.Contains(techID);
    }
    public void Load()
    {
        if (unlocked && !TechTree.UnlockedTechs.Contains(techID))
        {
            TechTree.UnlockedTechs.Add(techID);
            TechTree.TechDB[techID].ApplyEffects();
        }
    }
}
