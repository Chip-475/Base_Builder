using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class buildModeHoverMenu : MonoBehaviour
{
    public static buildModeHoverMenu instance;
    public GameObject hoverMenu{get {return this.gameObject;}}
    public GameObject entry;
    void Awake()
    {
        instance=this;
        switchState();
    }
    public void setup(BuildingData building,Vector3 pos)
    {
        foreach(Transform child in hoverMenu.transform) Destroy(child.gameObject);
        hoverMenu.transform.position=pos;
        foreach(var item in building.cost)
        {
            GameObject inst=Instantiate(entry,hoverMenu.transform,false);
            inst.GetComponent<Image>().sprite = item.Key.Sprite;
            inst.GetComponentInChildren<TMP_Text>().text = item.Value.ToString();
        }
    }
    public void switchState()
    {
        hoverMenu.SetActive(!hoverMenu.activeSelf);
    }
}
