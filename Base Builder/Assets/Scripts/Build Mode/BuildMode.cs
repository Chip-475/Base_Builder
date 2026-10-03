using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Cysharp.Threading.Tasks.Triggers;

public class BuildMode : MonoBehaviour
{
    public static BuildMode Instance { get; private set; }
    public static bool IsActive => Instance.panel.activeSelf;

    [Header("Setup")]
    [SerializeField] GameObject panel;
    [SerializeField] Button toggleButton;
    public List<BuildModeEntry> buildableObjects = new();

    public BuildModeEntry SelectedEntry { get; private set; }
    SpriteRenderer buildingPreview;

    private void Awake()
    {
        Instance = this;

        toggleButton.onClick.AddListener(() => Toggle());
        InitPreview();
        buildingPreview.enabled = false;
    }
    private void Start()
    {
        panel.SetActive(false);
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
    }
    void Disable()
    {
        panel.SetActive(false);
        buildingPreview.enabled = false;

        PlayerManager.Inputs.BuildMode.Disable();
        PlayerManager.Inputs.BuildMode.MouseMoved.performed -= (_) => Hover();
        PlayerManager.Inputs.BuildMode.LeftClick.performed -= (_) => Build();
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
            buildingPreview.sprite = SelectedEntry.icon;

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
    public void SetSelectedObject(BuildModeEntry obj)
    {
        SelectedEntry = obj;
    }
}
