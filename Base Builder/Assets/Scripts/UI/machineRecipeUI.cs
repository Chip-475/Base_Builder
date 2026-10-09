using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class machineRecipeUI : MonoBehaviour
{
    [Header("panello principale")]
    public static machineRecipeUI instance;
    public GameObject panello;
    public Transform cont;
    public GameObject prefabRic; //forse usare quello dello scroll view
    //public Sprite imgPrefab;
    //public recipeDataUI det;
    [Header("reference per quando clicci")]
    public TMP_Text titolo;
    public TMP_Text desc;
    public Image imgClick;
    public bool aperto
    {
        get
        {
            return panello.activeSelf;
        }
    }

    void Awake()
    {
        instance = this;
    }
    
    private Sprite getSprite(RecipeSO recipe)
    {
        if (recipe.outputResources.Length > 0 && recipe.outputResources[0].Sprite != null) return recipe.outputResources[0].Sprite;
        Debug.Log("niente sprite");
        return null;
    }

    public void apri(Machine machine)
    {
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(cont as RectTransform);
        Debug.Log("dentro la macchina");
        pulisciLista();
        panello.SetActive(true);
        Debug.Log(machine.Data.usableRecipes.Count);
        Debug.Log("dopo il count");
        foreach(RecipeSO recipe in machine.Data.usableRecipes)
        {
            Debug.Log("dentro il for");
            Debug.Log(recipe.r_name);
            GameObject voce=Instantiate(prefabRic, cont, false);
            Debug.Log(recipe.r_name);
            TMP_Text titolo = voce.transform.Find("panel/textTitolo").GetComponentInChildren<TMP_Text>();
            titolo.text = recipe.r_name;
            TMP_Text descri=voce.transform.Find("panel/desc").GetComponentInChildren<TMP_Text>();
            descri.text = "Ingredienti: ";
            foreach (ResourceSO r in recipe.inputResources)
            {
                descri.text = descri.text + r.Name + " ";
            }
            descri.text = descri.text+"\n" + "Risultato: ";
            foreach (ResourceSO r in recipe.outputResources)
            {
                descri.text = descri.text + r.Name + " ";
            }
            Image imageVoce = voce.transform.Find("panel/Image").GetComponent<Image>();
            imageVoce.sprite = getSprite(recipe);
            Button bott=voce.GetComponentInChildren<Button>();
            bott.onClick.AddListener(()=>mostraDett(recipe));
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
        Debug.Log("dentro mostra");
        titolo.text = recipe.r_name;
        desc.text = "Ingredienti: ";
        foreach(ResourceSO r in recipe.inputResources)
        {
            desc.text = desc.text + r.Name+" ";
        }
        desc.text =desc.text+"\n"+"Risultato: ";
        foreach (ResourceSO r in recipe.outputResources)
        {
            desc.text = desc.text + r.Name + " ";
        }
        imgClick.sprite = getSprite(recipe);
    }
}
