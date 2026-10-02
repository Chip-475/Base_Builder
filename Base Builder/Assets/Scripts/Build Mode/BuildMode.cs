using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class BuildMode : MonoBehaviour
{
    public static BuildMode Instance { get; private set; }
    public static bool IsActive => Instance.panel.activeSelf;

    [Header("Setup")]
    [SerializeField] GameObject panel;
    [SerializeField] Button toggleButton;

    public List<BuildingView> buildableObjects = new();
    public BuildingView SelectedObject;
    /*
    public Building SelectedObject { get; private set; }
    public BuildModeEntry oggetto;*/
    private void Awake()
    {
        Instance = this;
        toggleButton.onClick.AddListener(() => Toggle());
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

        PlayerManager.Inputs.BuildMode.Enable();
        PlayerManager.Inputs.BuildMode.MouseMoved.performed += (_) => OnHover();
        PlayerManager.Inputs.BuildMode.LeftClick.performed += (_) => Build();
    }
    void Disable()
    {
        panel.SetActive(false);

        PlayerManager.Inputs.BuildMode.Disable();
        PlayerManager.Inputs.BuildMode.MouseMoved.performed -= (_) => OnHover();
        PlayerManager.Inputs.BuildMode.LeftClick.performed -= (_) => Build();
    }

    void OnHover()
    {
        // Hover logic
    }
    void Build()
    {
        Debug.Log("dentro la build");
        if (SelectedObject == null) return;
        Debug.Log("dopo l'if");
        Vector3Int mousePos = Helpers.GetMousePosition().ToVector3Int();
        BuildingView view = Instantiate(SelectedObject, mousePos,Quaternion.identity);
        view.Init();
        Debug.Log($"View type: :{view.GetType()},Data: {view.Data}, building: {view.bulding}");
        if (view.bulding == null)
        {
            Debug.Log("non costruito bene");
            Destroy(view.gameObject);
            return;
        }
        var bouds = view.bulding.GetBounds();
        bouds.center = mousePos;
        var cells = view.bulding.GetCellsInBounds(bouds);
        Debug.Log($"mousePos: {mousePos},bounds: {bouds},celle: {cells.Length} ");
        foreach(var cell in cells)
        {
            Debug.Log($"cella {cell} canBuildOn: {cell.canBuildOn}");
        }
        if(!CanBuildOn(view.bulding.GetCellsInBounds(bouds)))
        {
            Debug.Log("CanBuildOn =false distrutto");
            Destroy(view.gameObject);
            return;
        }
        view.bulding.SetPosition(mousePos);
        SetSelectedObject(null);
    }
    
    public void SetSelectedObject(BuildingView obj)
    {
        SelectedObject = obj;
    }
    public static bool CanBuildOn(Cell[] cells)
    {
        foreach (var cell in cells)
            if (!cell.canBuildOn) return false;

        return true;
    }
}
/*
if (SelectedObject == null) return;
BuildingView prefabView = oggetto.building;
Vector3Int mousePos = Helpers.GetMouseWorldPosition().ToVector3Int();
BuildingView view = Instantiate(prefabView, mousePos, Quaternion.identity);
Building build = null;
if (view is MachineView machineView && view.Data is MachineData machineData) build = new Machine(machineData, machineView);
if (build == null)
{
    Destroy(view.gameObject);
    Debug.Log("non costruito bene");
    return;
}
var bounds = build.GetBounds();
bounds.center = mousePos;
if (!CanBuildOn(build.GetCellsInBounds(bounds)))
{
    Destroy(view.gameObject);
    return;
}
build.SetPosition(mousePos);
/*
var bounds = SelectedObject.GetBounds();
bounds.center = mousePos.ToVector3Int();
if (!CanBuildOn(SelectedObject.GetCellsInBounds(bounds)))
    return;

Machine obj = new Machine(SelectedObject.Data,SelectedObject.SceneObj);
obj.SetPosition(WorldManager.World.GetCellAt(Helpers.GetMouseWorldPosition().ToVector3Int()));

//SetSelectedObject(null);*/