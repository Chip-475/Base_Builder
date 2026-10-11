using UnityEngine;
using UnityEngine.UI;

public class MachineUI : MonoBehaviour
{
    Machine currentSelected;
    public Machine Selected
    {
        get { return currentSelected; }
        set { currentSelected = value; Build(value); }
    }
    public bool IsActive => gameObject.activeSelf;

    [Header("Setup")]
    [SerializeField] RecipeEntry recipeEntryPrefab;

    [Header("Right side")]
    [SerializeField] GameObject recipeListContent;

    [Header("Upper Left Side")]
    [SerializeField] Button backButton;
    [SerializeField] Image recipeIcon;

    [Header("Lower Left Side")]
    [SerializeField] Image craftedItemIcon;

    private void Awake()
    {
        gameObject.SetActive(false);

        backButton.onClick.AddListener(() => gameObject.SetActive(false));
    }

    public void Build(Machine machine)
    {
        Wipe();

        // Right Side
        foreach (var recipe in machine.Data.usableRecipes)
        {
            RecipeEntry entry = Instantiate(recipeEntryPrefab, recipeListContent.transform);
            entry.Build(recipe);
        }
    }

    void Wipe()
    {
        foreach (Transform child in recipeListContent.transform)
            Destroy(child.gameObject);
    }
}
