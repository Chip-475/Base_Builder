using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class recipeDataUI : MonoBehaviour
{
    [Header("info")]
    public Image icona;
    public TMP_Text nomeTesto;
    public TMP_Text descrizione;
    public TMP_Text tempo;

    [Header("ingredienti e prodotto")]
    public Transform contenuto;
    public GameObject prefabIngre;
    public Image iconeProd;
    public TMP_Text quant;

    public void mostra(RecipeSO recipe)
    {
        gameObject.SetActive(true);
        if (recipe.outputResources.Length > 0) icona.sprite = recipe.outputResources[0].r_sprite;
        nomeTesto.text = recipe.r_name;
        tempo.text = tempo.text+" "+recipe.completionTime + "s";
        pulisciIngredienti();
        for(int i=0;i<recipe.inputResources.Length;i++)
        {
            ResourceSO ris = recipe.inputResources[i];
            int quant = recipe.input[i];
            GameObject slot = Instantiate(prefabIngre, contenuto);
            slotRisorsa slotUI = slot.GetComponent<slotRisorsa>();
            if (recipe.outputResources.Length > 0) iconeProd.sprite = recipe.outputResources[0].r_sprite;
        } 
        if(recipe.outputResources.Length>0)
        {
            iconeProd.sprite = recipe.outputResources[0].r_sprite;
            quant.text = "x " + recipe.output[0];
        }

    }

    void pulisciIngredienti()
    {
        for(int i=contenuto.childCount-1;i>=0;i--)
        {
            Destroy(contenuto.GetChild(i).gameObject);
        }
    }
}
