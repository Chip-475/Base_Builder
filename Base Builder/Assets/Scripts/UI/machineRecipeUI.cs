using TMPro;
using UnityEngine;
public class machineRecipeUI : MonoBehaviour
{
    [Header("panello principale")]
    public static machineRecipeUI instance;
    public GameObject panello;
    public Transform cont;
    public GameObject prefabRic; //forse usare quello dello scroll view
    //public recipeDataUI det;
    [Header("reference per quando clicci")]
    public TMP_Text titolo;
    public TMP_Text desc;
    void Awake()
    {
        instance = this;
    }

    public void apri(Machine machine)
    {
        Debug.Log("dentro la macchina");
        pulisciLista();
        panello.SetActive(true);
        Debug.Log(machine.allowedRecipes.Count);
        foreach(RecipeSO recipe in machine.allowedRecipes)
        {
            Debug.Log("dentro il for");
            GameObject voce=Instantiate(prefabRic, cont, false);
            TMP_Text titolo = voce.transform.Find("textTitolo").GetComponent<TMP_Text>();
            titolo.text = recipe.r_name;
            TMP_Text descri=voce.transform.Find("desc").GetComponent<TMP_Text>();
            descri.text = "Ingredienti: " + "\n";
            foreach (ResourceSO r in recipe.inputResources)
            {
                descri.text = descri.text + r.r_name + " ";
            }
            descri.text = "\n" + "Risultato: " + "\n";
            foreach (ResourceSO r in recipe.outputResources)
            {
                descri.text = descri.text + r.r_name + " ";
            }
        }
    }
    
    private void pulisciLista()
    {
        for(int i=cont.childCount-1; i>=0; i--)
        {
            Destroy(cont.GetChild(i).gameObject);
        }
    }
    public void apriSelect()
    {
        //mostraDett(recipe);
    }
    public void chiudi()
    {
        panello.SetActive(false);
    }
    public void mostraDett(RecipeSO recipe)
    {
        titolo.text = recipe.r_name;
        desc.text = "Ingredienti: " + "\n";
        foreach(ResourceSO r in recipe.inputResources)
        {
            desc.text = desc.text + r.r_name+" ";
        }
        desc.text ="\n"+"Risultato: "+"\n";
        foreach(ResourceSO r in recipe.outputResources)
        {
            desc.text = desc.text + r.r_name+" ";
        }

    }
}
