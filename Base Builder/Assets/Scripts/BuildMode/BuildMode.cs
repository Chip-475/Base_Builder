using UnityEngine;
using System.Collections.Generic;
public class BuildMode : MonoBehaviour
{
    public static BuildMode Instance {  get; private set; }

    public List<InstalledObject> buildableObjects;
    private InstalledObject selectedObject;
    public InstalledObject SelectedObject
    {
        get { return selectedObject; }
        set
        {
            selectedObject = value;
        }
    }
    
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        gameObject.SetActive(Player.Instance.buildMode);
    }
    private void Update()
    {
        if (SelectedObject!=null)
        {
            Debug.Log("Selected Object: " + SelectedObject.name);
            HoveringPhase();
        }
    }
    public void HoveringPhase()
    {
        Cell mouseCell = WorldManager.instance.GetCellCoordsFromMouse();
        if (IsBuildable(SelectedObject))
        {
            SelectedObject.SetColor(Color.green);
        }
        else
        {
            SelectedObject.SetColor(Color.red);
        }
        SelectedObject.SetPosition(mouseCell);
    }
    public void Build()
    {
        if (!Player.Instance.buildMode) return;
        
    }
    public bool IsBuildable(InstalledObject obj)
    {
        foreach (var cell in obj.GetCellsInBounds())
        {
            if (!cell.canPlaceOn)
            {
                return false;
            }
        }
        return true;
    }
}
