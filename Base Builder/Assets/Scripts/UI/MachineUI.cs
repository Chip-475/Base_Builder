using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
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
        panel.SetActive(true);
        BuildRecipeList(machine);
    }
    public void BuildRecipeList(Machine machine)
    {
        WipeRecipeList();
        foreach(var recipe in machine.Data.usableRecipes)
        {
            RecipeEntry entry = Instantiate(recipeEntryPrefab, recipeListContent.transform);
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