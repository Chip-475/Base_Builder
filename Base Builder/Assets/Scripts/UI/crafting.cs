using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.Collections.Generic;
using UnityEngine.Rendering;

public class crafting : MonoBehaviour
{
    [Header("zone")]
    public RectTransform colonnaInput;
    public RectTransform livelloLinee;
    [Header("output")]
    public RectTransform slotoutput;
    public Image icona;
    public TMP_Text quant;
    [Header("prefab")]
    public GameObject prefabInput;
    [Header("linee")]
    public Color coloreLinea = Color.white;
    public float spessore = 4f;

    public void mostra(RecipeSO recipe)
    {
        if (recipe == null) return;
        pulisciFigli(colonnaInput);
        pulisciFigli(livelloLinee);
        List<RectTransform> slotCreati = new List<RectTransform>();
        foreach(KeyValuePair<ResourceSO,int> ingredienti in recipe.inputs)
        {
            GameObject slot = Instantiate(prefabInput, colonnaInput);
            Image icon=slot.GetComponent<Image>();
            icon.sprite = ingredienti.Key.Sprite;
            TMP_Text testo=slot.GetComponent<TMP_Text>();
            testo.text = "x" + ingredienti.Value;
            slotCreati.Add(slot.GetComponent<RectTransform>());
        }
        bool output = false;
        foreach(KeyValuePair<ResourceSO,int> prod in recipe.outputs)
        {
            icona.sprite = prod.Key.Sprite;
            quant.text = "x" + prod.Value;
            output = true;
            break;
        }
        icona.gameObject.SetActive(output);
        if(output)
        {
            foreach(RectTransform slot in slotCreati)
            {
                disegnaLinea(slot, slotoutput);
            }
        }
        //Canvas.ForceUpdateCanvases();
        //LayoutRebuilder.ForceRebuildLayoutImmediate(colonnaInput);
        
    }

    private void disegnaLinea(RectTransform da,RectTransform a)
    {
        Vector3 puntoDa = da.TransformPoint(new Vector3(da.rect.xMax, da.rect.center.y, 0));
        Vector3 puntoA = a.TransformPoint(new Vector3(a.rect.xMin, a.rect.center.y, 0));
        Vector2 inizio = livelloLinee.InverseTransformPoint(puntoDa);
        Vector2 fine = livelloLinee.InverseTransformPoint(puntoA);
        Vector2 direzione = fine - inizio;

        GameObject ogg = new GameObject("Linea", typeof(RectTransform), typeof(Image));
        ogg.transform.SetParent(livelloLinee, false);
        Image img = ogg.GetComponent<Image>();
        img.color = coloreLinea;
        img.raycastTarget = false;

        RectTransform linea = ogg.GetComponent<RectTransform>();
        linea.anchorMin = new Vector2(0.5f, 0.5f);
        linea.anchorMax = new Vector2(0.5f, 0.5f);
        linea.pivot = new Vector2(0.5f, 0.5f);
        linea.sizeDelta = new Vector2(direzione.magnitude, spessore);
        linea.localPosition = (inizio + fine) / 2f;
        linea.localRotation = Quaternion.Euler(0, 0, Mathf.Atan2(direzione.y, direzione.x)*Mathf.Rad2Deg);
    }


    private void pulisciFigli(Transform gen)
    {
        for(int i=gen.childCount-1;i>=0;i++)
        {
            GameObject figlio = gen.GetChild(i).gameObject;
            figlio.SetActive(false);
            Destroy(figlio);
        }
    }
}
