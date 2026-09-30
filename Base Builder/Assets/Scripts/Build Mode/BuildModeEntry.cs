using UnityEngine;
using UnityEngine.UI;

public class BuildModeEntry : MonoBehaviour
{/*
    [SerializeField] Image image;
    [SerializeField] Button button;
    public Sprite icon;

    public BuildingView building;
    */
    public GameObject prefabMachine;
    private void Awake()
    {
        //image.sprite = icon;
        Button b = prefabMachine.GetComponent<Button>();
        b.onClick.AddListener(() => OnClick());
    }

    public void OnClick()   
    {
        Debug.Log("Dentro il click");
        BuildMode.Instance.SetSelectedObject(this);
        BuildMode.Instance.click();
        //to add:selected object sprite sparkle effect
    }
}
