using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.Collections.Generic;
using UnityEngine.Rendering;
using Mono.Cecil;
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
        pulisciFigli(colonnaInput);
        pulisciFigli(livelloLinee);
        List<RectTransform> slotCreati = new List<RectTransform>();
        for(int i=0;i<recipe.inputResources.Length;i++)
        {
            ResourceSO ris=recipe.inputResources[i];
            GameObject slot = Instantiate(prefabInput, colonnaInput);
            Image icon=slot.GetComponent<Image>();
            icon.sprite = ris.Sprite;
            TMP_Text testo=slot.GetComponent<TMP_Text>();
            testo.text = "x" + recipe.input[i];
            slotCreati.Add(slot.GetComponent<RectTransform>());
        }
        if (recipe.outputResources.Length > 0)
        {
            icona.gameObject.SetActive(true);
            icona.sprite = recipe.outputResources[0].Sprite;
            quant.text = "x" + recipe.output[0];
        }
        else icona.gameObject.SetActive(false);
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(colonnaInput);
        foreach(RectTransform slot in slotCreati)
        {
            disegnaLinea(slot, slotoutput);
        }
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
