using UnityEngine;
using UnityEngine.UI;

public class BuildModeEntry : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] Button button;
    public Sprite icon;

    public BuildingView buildingPrefab;

    private void Awake()
    {
        image.sprite = icon;
        button.onClick.AddListener(() => OnClick());
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
