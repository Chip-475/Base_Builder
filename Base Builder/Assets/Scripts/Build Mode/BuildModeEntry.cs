using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuildModeEntry : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Image image;
    [SerializeField] Image background;
    [SerializeField] Button button;

    public BuildingView buildingPrefab;
    public Dictionary<ResourceSO, int> Cost => buildingPrefab.Data.cost;

    private void Awake()
    {
        image.sprite = buildingPrefab.Data.sprite;
        button.onClick.AddListener(() => OnClick());
    }
    void Update()
    {
        CanBuild();
    }

    public void CanBuild()
    {
        var buildingsOfOwnType = BuildingManager.Buildings.Values.Where(b => b.Data.ID == buildingPrefab.Data.ID);

        if (buildingPrefab.Data.ID == "power_pole_mk1")
            if (button.interactable = buildingsOfOwnType.Count() < 5)
                button.interactable = true;
            else
                button.interactable = Depot.Inventory.CanRemoveItems(Cost);
        else
            if (button.interactable = buildingsOfOwnType.Count() < 1)
                button.interactable = true;
            else
                button.interactable = Depot.Inventory.CanRemoveItems(Cost);
    }

    public void OnClick()
    {
        if (buildingPrefab == null || buildingPrefab.Data == null)
        {
            Debug.LogError($"Build mode entry '{name}' has no valid building prefab assigned.", this);
            return;
        }

        BuildMode.Instance.SetSelectedEntry(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(buildModeHoverMenu.instance==null||buildingPrefab==null||buildingPrefab.Data==null)return;
        Vector3 pos=transform.position+new Vector3(0,100,0);
        buildModeHoverMenu.instance.Show(buildingPrefab.Data,pos);
        //buildModeHoverMenu.instance.switchState();
        //buildModeHoverMenu.instance.setup(buildingPrefab.Data,new Vector3(transform.position.x,transform.position.y+100,transform.position.z));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //buildModeHoverMenu.instance.switchState();
        if(buildModeHoverMenu.instance!=null)buildModeHoverMenu.instance.Hide();
    }

    void Onsable()
    {
        if(buildModeHoverMenu.instance!=null)buildModeHoverMenu.instance.Hide();
    }
}
