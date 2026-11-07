using UnityEngine;
using UnityEngine.UI;

public class InventoryMenu : MonoBehaviour
{
    public static InventoryMenu Instance { get; private set; }

    [Header("Setup")]
    [SerializeField] Image background;
    [SerializeField] ItemEntry itemPrefab;
    //[SerializeField] Button closeButton;
    [SerializeField] Button discardModeButton;
    [Space]
    [SerializeField] Transform itemListContent;

    [Header("Runtime")]
    public bool discardMode;

    private void Awake()
    {
        Instance = this;

        //closeButton.onClick.AddListener(Toggle);
        discardModeButton.onClick.AddListener(ToggleDiscardMode);
        PlayerManager.Inputs.Player.OpenInventory.performed += (_) => Toggle();

        gameObject.SetActive(false);
    }
    private void Update()
    {
        if (discardMode)
            background.color = Color.softRed;
        else
            background.color = Color.white;
    }

    public void Toggle()
    {
        gameObject.SetActive(!gameObject.activeSelf);

        WipeList();
        foreach (var item in Depot.Inventory.Items.Keys)
            Instantiate(itemPrefab, itemListContent).Build(item);

        discardMode = false;
    }

    void WipeList()
    {
        foreach (Transform child in itemListContent)
            Destroy(child.gameObject);
    }
    void ToggleDiscardMode() => discardMode = !discardMode;
}
