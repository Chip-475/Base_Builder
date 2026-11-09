using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class buildModeHoverMenu : MonoBehaviour
{
    public static buildModeHoverMenu instance;
    public GameObject hoverMenu{get {return this.gameObject;}}
    public GameObject panel;
    public GameObject entry;
    void Awake()
    {
        instance=this;
        //switchState();
        panel.SetActive(false);
    }
    public void Show(BuildingData building,Vector3 pos)
    {
       if(building==null||building.cost==null) return;
       for(int i=panel.transform.childCount-1;i>=0;i--)
       {
            GameObject vecchia=panel.transform.GetChild(i).gameObject;
            vecchia.SetActive(false);
            Destroy(vecchia);
       }
       panel.transform.position=pos;
       foreach(KeyValuePair<ResourceSO,int> item in building.cost)
       {
            GameObject riga=Instantiate(entry,panel.transform,false);
            Image icon=riga.GetComponent<Image>();
            if(icon!=null)icon.sprite=item.Key.Sprite;
            else Debug.Log("non ha un imagine dio porco");
            TMP_Text testo=riga.GetComponentInChildren<TMP_Text>(true);
            if(testo!=null)testo.text=item.Value.ToString();
            else Debug.Log("la riga non ha un costo cane dio");
       }
       panel.SetActive(true);
    }
    public void Hide()
    {
        panel.SetActive(false);
    }
    /*
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
    }*/
    /*
    public void switchState()
    {
        hoverMenu.SetActive(!hoverMenu.activeSelf);
    }*/
}
