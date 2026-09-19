using UnityEngine;
using System.Collections.Generic;
public class BuildMode : MonoBehaviour
{
    public List<InstalledObject> buildableObjects;
    private InstalledObject so;
    public InstalledObject selectedObject
    {
        get { return so; }
        set
        {
            so = value;
        }
    }
    public static BuildMode instance;
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        gameObject.SetActive(Player.instance.buildMode);
    }
    private void Update()
    {
        if (selectedObject!=null)
        {
            Debug.Log("Selected Object: " + selectedObject.name);
            HoveringPhase();
        }
    }
    public void HoveringPhase()
    {
        Cell mouseCell = WorldManager.instance.GetCellCoordsFromMouse();
        if (IsBuildable(selectedObject))
        {
            selectedObject.SetColor(Color.green);
        }
        else
        {
            selectedObject.SetColor(Color.red);
        }
        selectedObject.SetPosition(mouseCell);
    }
    public void Build()
    {
        if (!Player.instance.buildMode) return;
        
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
