using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class recipeDataUI : MonoBehaviour
{
    [Header("info")]
    public Image icona;
    public Text nomeTesto;
    public Text descrizione;
    public Text tempo;

    [Header("ingredienti e prodotto")]
    public Transform contenuto;
    public GameObject prefabIngre;
    public Image iconeProd;
    public Text quant;

    public void mostra(RecipeSO recipe)
    {
        gameObject.SetActive(true);
        //icona.sprite = recipe.sprite;
        nomeTesto.text= recipe.r_name;
        descrizione=recipe.GetComponent<Text>();
        tempo.text = recipe.completionTime + "s";
        pulisciIngredienti();
        /*foreach(ResourceAmount ingre in recipe.inputResources)
        {
        /
        }*/
    }

    void pulisciIngredienti()
    {
        for(int i=contenuto.childCount-1;i>=0;i--)
        {
            Destroy(contenuto.GetChild(i).gameObject);
        }
    }
}
