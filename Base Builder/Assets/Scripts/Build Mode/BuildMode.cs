using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Cysharp.Threading.Tasks.Triggers;
using Unity.VisualScripting;
using System.Linq;

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

    List<BuildModeEntry> allEntries = new();
    public BuildModeEntry SelectedEntry { get; private set; }
    SpriteRenderer buildingPreview;

    private void Awake()
    {
        Instance = this;

        toggleButton.onClick.AddListener(() => { Toggle(); SortBy(BuildingType.Machine); });
        machineSort.onClick.AddListener(() => SortBy(BuildingType.Machine));
        miningSort.onClick.AddListener(() => SortBy(BuildingType.Mining));
        powerSort.onClick.AddListener(() => SortBy(BuildingType.Power));
        storageSort.onClick.AddListener(() => SortBy(BuildingType.Storage));
        InitPreview();
        buildingPreview.enabled = false;
    }
    private void Start()
    {
        panel.SetActive(false);
        allEntries = content.GetComponentsInChildren<BuildModeEntry>().ToList();
    }

    public void Toggle()
    {
        Debug.Log("Toggle");
        panel.SetActive(!IsActive);

        if (IsActive) Enable();
        else Disable();
    }
    void Enable()
    {
        panel.SetActive(true);
        buildingPreview.enabled = true;

        PlayerManager.Inputs.BuildMode.Enable();
        PlayerManager.Inputs.BuildMode.MouseMoved.performed += (_) => Hover();
        PlayerManager.Inputs.BuildMode.LeftClick.performed += (_) => Build();
        PlayerManager.Inputs.BuildMode.RightClick.performed += (_) =>
        {
            SetSelectedEntry(null);
            buildingPreview.sprite = null;
        };
    }
    void Disable()
    {
        panel.SetActive(false);
        buildingPreview.enabled = false;

        PlayerManager.Inputs.BuildMode.Disable();
        PlayerManager.Inputs.BuildMode.MouseMoved.performed -= (_) => Hover();
        PlayerManager.Inputs.BuildMode.LeftClick.performed -= (_) => Build();
        PlayerManager.Inputs.BuildMode.RightClick.performed -= (_) =>
        {
            SetSelectedEntry(null);
            buildingPreview.sprite = null;
        };
    }

    void Hover()
    {
        Vector3Int mouseGridPos = Helpers.GetMouseWorldPosition().ToVector3Int();
        if (SelectedEntry == null)
        {
            buildingPreview.sprite = null;
            return;
        }
        else
            buildingPreview.sprite = SelectedEntry.buildingPrefab.Data.sprite;
        buildingPreview.transform.position = mouseGridPos;

        if (SelectedEntry.buildingPrefab.CanBuildOn(mouseGridPos))
            buildingPreview.color = Color.green;
        else
            buildingPreview.color = Color.red;
    }
    void Build()
    {
        Vector3Int mouseGridPos = Helpers.GetMouseWorldPosition().ToVector3Int();
        if (SelectedEntry == null)
            return;
        if (!SelectedEntry.buildingPrefab.CanBuildOn(mouseGridPos))
            return;

        Instantiate(SelectedEntry.buildingPrefab, mouseGridPos, Quaternion.identity);
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
