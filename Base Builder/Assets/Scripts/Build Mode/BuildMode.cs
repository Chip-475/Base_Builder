using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;

public class BuildMode : MonoBehaviour
{
    public static BuildMode Instance { get; private set; }
    public static bool IsActive => Instance.panel.activeSelf;

    [Header("Setup")]
    [SerializeField] GameObject panel;
    [SerializeField] GameObject content;
    [SerializeField] Button toggleButton;
    [Space]
    [SerializeField] Button machineSort;
    [SerializeField] Button miningSort;
    [SerializeField] Button storageSort;
    [SerializeField] Button powerSort;
    [Space]
    [SerializeField] Button destroyMode;

    List<BuildModeEntry> allEntries = new();
    public BuildModeEntry SelectedEntry { get; private set; }
    public Building SelectedBuilding { get; private set; }
    SpriteRenderer buildingPreview;
    bool destroying;

    private void Awake()
    {
        Instance = this;

        // UI
        toggleButton.onClick.AddListener(() => { Toggle(); SortBy(BuildingType.Machine); destroying = false; });
        machineSort.onClick.AddListener(() => { SortBy(BuildingType.Machine); destroying = false; });
        miningSort.onClick.AddListener(() => { SortBy(BuildingType.Mining); destroying = false; });
        powerSort.onClick.AddListener(() => { SortBy(BuildingType.Power); destroying = false; });
        storageSort.onClick.AddListener(() => { SortBy(BuildingType.Storage); destroying = false; });
        destroyMode.onClick.AddListener(() => { SortBy(BuildingType.None); destroying = true; });

        InitPreview();
        buildingPreview.enabled = false;
    }
    private void Start()
    {
        panel.SetActive(false);
        allEntries = content.GetComponentsInChildren<BuildModeEntry>().ToList();

        // Inputs
        PlayerManager.Inputs.BuildMode.MouseMoved.performed += (_) =>
        {
            if(!destroying) BuildHover();
            else DestroyHover();
        };
        PlayerManager.Inputs.BuildMode.LeftClick.performed += (_) =>
        {
            if (!destroying) Build();
            else Destroy();
        };
        PlayerManager.Inputs.BuildMode.RightClick.performed += (_) =>
        {
            SetSelectedEntry(null);
            buildingPreview.sprite = null;
        };
    }

    public void Toggle()
    {
        if (IsActive) Disable();
        else Enable();
    }
    void Enable()
    {
        panel.SetActive(true);
        buildingPreview.enabled = true;

        PlayerManager.Inputs.BuildMode.Enable();
    }
    void Disable()
    {
        panel.SetActive(false);
        buildingPreview.sprite = null;
        SelectedEntry = null;

        PlayerManager.Inputs.BuildMode.Disable();
    }

    void BuildHover()
    {
        Vector3Int mouseGridPos = Helpers.GetMouseWorldPosition().ToVector3Int();
        if (SelectedEntry == null || Helpers.IsMouseOverUI())
        {
            buildingPreview.sprite = null;
            return;
        }
        else
            buildingPreview.sprite = SelectedEntry.buildingPrefab.Data.sprite;

        buildingPreview.transform.position = mouseGridPos - SelectedEntry.buildingPrefab.Data.bounds.center;

        if (SelectedEntry.buildingPrefab.CanBuildOn(mouseGridPos))
            buildingPreview.color = Color.green;
        else
            buildingPreview.color = Color.red;
    }
    void DestroyHover()
    {
        Vector3Int mouseGridPos = Helpers.GetMouseWorldPosition().ToVector3Int();
        SelectedBuilding = WorldManager.World.GetCellAt(mouseGridPos).Building;
    }
    void Build()
    {
        Vector3Int mouseGridPos = Helpers.GetMouseWorldPosition().ToVector3Int();
        if (SelectedEntry == null || Helpers.IsMouseOverUI())
            return;
        if (!SelectedEntry.buildingPrefab.CanBuildOn(mouseGridPos))
            return;
        Depot.Inventory.RemoveItems(SelectedEntry.requiredResources);
        Instantiate(SelectedEntry.buildingPrefab, mouseGridPos - SelectedEntry.buildingPrefab.Data.bounds.center, Quaternion.identity);
    }
    void Destroy()
    {
        SelectedBuilding?.Destroy();
    }

    void InitPreview()
    {
        GameObject go = new();
        buildingPreview = go.AddComponent<SpriteRenderer>();
    }
    public void SetSelectedEntry(BuildModeEntry obj)
    {
        SelectedEntry = obj;
    }
    public void SortBy(BuildingType type)
    {
        List<BuildModeEntry> entries = new(allEntries);
        List<BuildModeEntry> requestedEntries = new();
        foreach (var entry in entries)
            if (entry.buildingPrefab.Data.type == type)
                requestedEntries.Add(entry);
        
        foreach (var entry in entries)
            entry.gameObject.SetActive(false);
        foreach (var entry in requestedEntries)
            entry.gameObject.SetActive(true);

        for (int i = 0; i < requestedEntries.Count; i++)
            requestedEntries[i].gameObject.transform.SetSiblingIndex(i);
    }
}
