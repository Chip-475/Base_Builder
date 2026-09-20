using Unity.VisualScripting;
using UnityEngine;

public class machineRecipeUI : MonoBehaviour
{
    public static machineRecipeUI instance;
    public GameObject panello;
    public Transform cont;
    public GameObject prefabRic; //forse usare quello dello scroll view
    public recipeDataUI det;

    void Awake()
    {
        instance = this;
    }

    public void apri(Machine machine)
    {
        pulisciLista();
        foreach(RecipeSO recipe in machine.allowedRecipes)
        {
            GameObject voce = Instantiate(prefabRic, cont);
            recipeListUI voce2 = voce.GetComponent<recipeListUI>();
            if(voce != null )
            {
                voce2.imposta(recipe, this);
            }
        }
        panello.SetActive(true);
    }
    
    private void pulisciLista()
    {
        for(int i=cont.childCount-1; i>=0; i--)
        {
            Destroy(cont.GetChild(i).gameObject);
        }
    }
    public void chiudi()
    {
        panello.SetActive(false);
    }
    public void mostraDett(RecipeSO recipe)
    {
        det.mostra(recipe);
    }
}
