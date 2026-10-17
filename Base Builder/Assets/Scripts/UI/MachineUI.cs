using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MachineUI : MonoBehaviour
{
    public static MachineUI Instance { get; private set; }

    public bool IsActive => panel.activeSelf;

    [Header("Setup")]
    [SerializeField] GameObject panel;
    [SerializeField] Button backButton;
    [SerializeField] RecipeEntry recipeEntryPrefab;

    [Header("Recipe List")]
    [SerializeField] GameObject recipeListContent;

    [Header("Info Panel")]
    [SerializeField] Image selectedRecipeIcon;
    [SerializeField] TMP_Text selectedRecipeName;
    [SerializeField] TMP_Text selectedRecipeDesc;

    [Header("Lower Right Side")]
    [SerializeField] List<Image> inputResourceSlots;
    [SerializeField] List<Image> outputResourceSlots;

    private void Awake()
    {
        Instance = this;

        backButton.onClick.AddListener(() => panel.SetActive(false));
    }

    public void Open(Machine machine)
    {
        if(machine==null)
        {
            Debug.Log("machine è null");
            return;
        }
        if(machine.Data==null)
        {
            Debug.Log("machine data è null");
            return;
        }
        Open(machine.Data.usableRecipes);
    }

    public void Open(List<RecipeSO> ricette)
    {
        if(ricette==null)
        {
            Debug.Log("lista null");
            return;
        }
        panel.SetActive(true);
        BuildRecipeList(ricette);
    }

    public void BuildRecipeList(List<RecipeSO>ricette)
    {
        WipeRecipeList();
        /*
        foreach(var recipe in machine.Data.usableRecipes)
        {
            RecipeEntry entry = Instantiate(recipeEntryPrefab, recipeListContent.transform);
            entry.Build(recipe);
        }
        */
        foreach(RecipeSO recipe in ricette)
        {
            if(recipe==null) continue;
            RecipeEntry entry=Instantiate(recipeEntryPrefab,recipeListContent.transform);
            entry.Build(recipe);
        }
    }
    public void BuildInfoPanel(RecipeEntry recipeEntry)
    {
        selectedRecipeIcon.sprite = recipeEntry.recipe.Sprite;
        selectedRecipeName.text = recipeEntry.recipe.Name;
        selectedRecipeDesc.text = recipeEntry.recipe.Desc;
    }
    public void BuildLowerRight()
    {

    }

    void WipeRecipeList()
    {
        foreach (Transform child in recipeListContent.transform)
            Destroy(child.gameObject);
    }
}