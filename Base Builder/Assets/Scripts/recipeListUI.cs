using UnityEngine;
using UnityEngine.UI;
public class recipeListUI : MonoBehaviour
{
    public Image icona;
    public Text nomeTesto;
    public Button bottone;

    RecipeSO recipe;
    machineRecipeUI menuPrincipale;

    public void imposta(RecipeSO recipe1,machineRecipeUI menu)
    {
        recipe = recipe1;
        menuPrincipale = menu;
        //if(icona!=null)icona.sprite=recipe.
        nomeTesto.text = recipe.r_name;
        
    }
    public void OnClick()
    {
        menuPrincipale.mostraDett(recipe);
    }
}
