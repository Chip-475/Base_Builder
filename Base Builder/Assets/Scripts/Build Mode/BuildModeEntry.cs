using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BuildModeEntry : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] Button button;
    public Sprite icon;

    [SerializeField] public Dictionary<ResourceSO,int> requiredResources=new();

    public BuildingView buildingPrefab;

private void Awake()
{
    if (requiredResources == null)
        requiredResources = new Dictionary<ResourceSO, int>();

    image.sprite = icon;
    button.onClick.AddListener(() => OnClick());
}
    void Update()
    {
        canBuild();
    }
public void canBuild()
{
    button.interactable = Depot.Inventory.CanRemoveItems(requiredResources);
}
    public void OnClick()
    {
        if (buildingPrefab == null || buildingPrefab.Data == null)
        {
            Debug.LogError($"Build mode entry '{name}' has no valid building prefab assigned.", this);
            return;
        }

        BuildMode.Instance.SetSelectedEntry(this);
        //to add:selected object sprite sparkle effect
    }
}
