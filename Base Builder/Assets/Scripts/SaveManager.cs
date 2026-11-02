using Newtonsoft.Json;
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

        savePath = Path.Combine(Application.persistentDataPath, "save.json");

        saveButton.onClick.AddListener(() => file.SaveAll(savePath));
        loadButton.onClick.AddListener(() => file.LoadFrom(savePath));

        #if !UNITY_EDITOR
            saveButton.gameObject.SetActive(false);
        #endif
    }
}

// anche questa
[Serializable]
public class SaveFile
{
    // To Save
    public PlayerSaveData playerData;
    public List<BuildingSaveData> buildingsData = new();
    public List<BotSaveData> botsData = new(); // tasks still not saved
    public DepotSaveData depotData;
    public List<TechSaveData> techsData = new();
    // - Settings

    public void SaveAll(string path)
    {
        playerData = new();

        foreach (var building in BuildingManager.Buildings.Values.Where(b => b is not MineralNode))
            buildingsData.Add(new(building));

        foreach (var bot in BotManager.Bots)
            botsData.Add(new(bot));

        depotData = new();
        
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

        foreach (var data in botsData)
            data.Load();

        depotData?.Load();

        foreach (var data in techsData)
            data?.Load();

        // settings
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

    public PlayerSaveData()
    {
        position = new(PlayerManager.Instance.gameObject.transform.position);
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
public class BotSaveData
{
    public string botID;
    public Helpers.SerializableVector3 position;
    public float power;
    // task

    public BotSaveData(Bot bot)
    {
        botID = bot.Data.ID;
        position = new(bot.Coords);
        power = bot.Power;
        //task
    }
    public void Load()
    {
        BotView prefab = SaveManager.Instance.botPrefabs.First(p => p.data.ID == botID);
        var bot = MonoBehaviour.Instantiate(prefab, position.ToVector3(), Quaternion.identity);

        bot.RuntimeObj.Power = power;
        // task
    }
}
[Serializable]
public class DepotSaveData
{
    [SerializeField] public Dictionary<string, int> items = new();

    public DepotSaveData()
    {
        foreach (var item in Depot.Inventory.Items)
            items.Add(item.Key.ID, item.Value);
    }
    public void Load()
    {
        foreach (var item in items)
            Depot.Inventory.AddItem(SaveManager.Instance.resourceDB.First(res => res.ID == item.Key), item.Value);
    }
}
[Serializable]
public class TechSaveData
{
    public string techID;
    public bool unlocked;

    public TechSaveData(TechSO tech)
    {
        techID = tech.id;
        unlocked = TechTree.UnlockedTechs.Contains(techID);
    }
    public void Load()
    {
        if (unlocked && !TechTree.UnlockedTechs.Contains(techID))
            TechTree.UnlockedTechs.Add(techID);
    }
}
