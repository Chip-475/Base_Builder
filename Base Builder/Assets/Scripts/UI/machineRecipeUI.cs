using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class machineRecipeUI : MonoBehaviour
{
    //[Header("panello principale")]
    //public static machineRecipeUI instance;
    //public GameObject panello;
    //public Transform cont;
    //public GameObject prefabRic; //forse usare quello dello scroll view
    ////public recipeDataUI det;
    //[Header("reference per quando clicci")]
    //public TMP_Text titolo;
    //public TMP_Text desc;
    //void Awake()
    //{
    //    instance = this;
    //}

    //public void apri(Machine machine)
    //{
    //    Debug.Log("dentro la macchina");
    //    pulisciLista();
    //    panello.SetActive(true);
    //    Debug.Log(machine.Data.usableRecipes.Count);
    //    foreach(RecipeSO recipe in machine.Data.usableRecipes)
    //    {
    //        Debug.Log("dentro il for");
    //        Debug.Log(recipe.Name);
    //        GameObject voce=Instantiate(prefabRic, cont, false);
    //        Debug.Log(recipe.Name);
    //        TMP_Text titolo = voce.transform.Find("Panel/textTitolo").GetComponentInChildren<TMP_Text>();
    //        titolo.text = recipe.Name;
    //        TMP_Text descri=voce.transform.Find("Panel/desc").GetComponentInChildren<TMP_Text>();
    //        descri.text = "Ingredienti: ";
    //        foreach (ResourceSO r in recipe.inputResources)
    //        {
    //            descri.text = descri.text + r.Name + " ";
    //        }
    //        descri.text = descri.text+"\n" + "Risultato: ";
    //        foreach (ResourceSO r in recipe.outputResources)
    //        {
    //            descri.text = descri.text + r.Name + " ";
    //        }
    //        Button bott=voce.GetComponentInChildren<Button>();
    //        bott.onClick.AddListener(()=>mostraDett(recipe));
    //    }
    //}
    
    //private void pulisciLista()
    //{
    //    for(int i=cont.childCount-1; i>=0; i--)
    //    {
    //        Destroy(cont.GetChild(i).gameObject);
    //    }
    //}
    //public void apriSelect()
    //{
    //    //mostraDett(recipe);
    //}
    //public void chiudi()
    //{
    //    panello.SetActive(false);
    //}
    //public void mostraDett(RecipeSO recipe)
    //{
    //    Debug.Log("dentro mostra");
    //    titolo.text = recipe.Name;
    //    desc.text = "Ingredienti: ";
    //    foreach(ResourceSO r in recipe.inputResources)
    //    {
    //        desc.text = desc.text + r.Name+" ";
    //    }
    //    desc.text =desc.text+"\n"+"Risultato: ";
    //    foreach (ResourceSO r in recipe.outputResources)
    //    {
    //        desc.text = desc.text + r.Name + " ";
    //    }
    //}
}
