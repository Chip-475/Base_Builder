using System.Collections.Generic;
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
        if (buildingPrefab != null && buildingPrefab.Data != null && image != null)
            image.sprite = buildingPrefab.Data.sprite;
        if (button != null)
            button.onClick.AddListener(() => OnClick());
    }

    public void Configure(BuildingView prefab)
    {
        buildingPrefab = prefab;
        if (image != null && buildingPrefab != null && buildingPrefab.Data != null)
            image.sprite = buildingPrefab.Data.sprite;
    }
    void Update()
    {
        CanBuild();
    }

    public void CanBuild()
    {
        if (button != null)
            button.interactable = Depot.Inventory != null && buildingPrefab != null
                && Depot.Inventory.CanRemoveItems(Cost);
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
