using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(Button))]
public class RecipeEntry : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] Button button;
    [SerializeField] Image craftedItem;
    [SerializeField] List<Image> requestedItems;

    readonly Dictionary<Image, TMP_Text> quantityText = new();

    private void Awake()
    {
        button = GetComponent<Button>();
        
        foreach (var item in requestedItems)
            quantityText[item] = item.GetComponentInChildren<TMP_Text>();
    }

    public void Build(RecipeSO recipe)
    {
        if (recipe.inputs.Count > 3 || recipe.outputs.Count > 1)
            throw new Exception($"Recipe {recipe.Name} is invalid.");

        for (int i = 0; i < recipe.inputs.Count; i++)
        {
            Image currentEntry = requestedItems[i];
            var Keys = recipe.inputs.Keys.ToArray();
            var Values = recipe.outputs.Keys.ToArray();

            currentEntry.sprite = Keys[i].Sprite;
            quantityText[currentEntry].text = Values[i].ToString();
        }
    }
}
