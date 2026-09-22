using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class recipeListUI : MonoBehaviour
{
    public Image icona;
    public TMP_Text nomeTesto;
    public Button bottone;

    RecipeSO recipe;
    machineRecipeUI menuPrincipale;

    public void imposta(RecipeSO recipe1,machineRecipeUI menu)
    {
        if (icona != null && recipe1.outputResources.Length > 0) icona.sprite = recipe1.outputResources[0].r_sprite;
        if (nomeTesto != null) nomeTesto.text = recipe1.r_name;
    }
    public void OnClick()
    {
        menuPrincipale.mostraDett(recipe);
    }
}
