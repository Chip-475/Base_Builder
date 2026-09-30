using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class BuildMode : MonoBehaviour
{
    public static BuildMode Instance {  get; private set; }
    public static bool IsActive => Instance.panel.activeSelf;

    [Header("Setup")]
    [SerializeField] GameObject panel;
    [SerializeField] Button toggleButton;

    public List<BuildingView> buildableObjects = new();
    public BuildModeEntry SelectedObject { get; private set; }
    
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
        if (SelectedObject == null)
        {
            Debug.Log("esco subito");
            return;
        }
        BuildingView buldingView = SelectedObject.prefabMachine.GetComponent<BuildingView>();
        if(buldingView==null)
        {
            Debug.Log("il prefab " + SelectedObject.prefabMachine.name + " non ha un componente");
            return;
        }
      /*  Debug.Log($"building: {SelectedObject}");
        Debug.Log($"Data: {SelectedObject.Data}");*/
        var mousePos = Helpers.GetMouseWorldPosition();
        var bounds = buldingView.Data.bounds;
        //var size = SelectedObject.building.Data.bounds.size;
        var center = mousePos.ToVector3Int();
        //var bounds2 = new BoundsInt(center - Vector3Int.zero,size);
        bounds.center = mousePos.ToVector3Int();
        if (!CanBuildOn(Building.GetCellsInBounds(bounds)))
           return;

        var obj = Instantiate(SelectedObject.prefabMachine);
        BuildingView objView=obj.GetComponent<BuildingView>();
        Cell cella = WorldManager.World.GetCellAt(Helpers.GetMouseWorldPosition().ToVector3Int());
        objView.Building.SetPosition(new Vector3Int(0,0,0));

        SetSelectedObject(null);
    }

    public void SetSelectedObject(BuildModeEntry obj)
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
