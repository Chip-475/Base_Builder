using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BuildModeEntry : MonoBehaviour
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

    public void CanBuild() => button.interactable = Depot.Inventory.CanRemoveItems(Cost);
    public void OnClick()
    {
        if (buildingPrefab == null || buildingPrefab.Data == null)
        {
            Debug.LogError($"Build mode entry '{name}' has no valid building prefab assigned.", this);
            return;
        }

        BuildMode.Instance.SetSelectedEntry(this);
    }
}
