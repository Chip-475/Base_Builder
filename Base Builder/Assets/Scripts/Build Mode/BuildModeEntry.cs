using UnityEngine;
using UnityEngine.UI;

public class BuildModeEntry : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] Button button;
    public Sprite icon;

    public BuildingView building;

    private void Awake()
    {
        image.sprite = icon;
        button.onClick.AddListener(() => OnClick());
    }

    public void OnClick()
    {
        BuildMode.Instance.SetSelectedObject(this);
        //to add:selected object sprite sparkle effect
    }
}
