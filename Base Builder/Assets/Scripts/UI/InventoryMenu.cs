using UnityEngine;
using UnityEngine.UI;

public class InventoryMenu : MonoBehaviour
{
    public static InventoryMenu Instance { get; private set; }

    [Header("Setup")]
    [SerializeField] ItemEntry itemPrefab;
    [SerializeField] Button closeButton;
    [Space]
    [SerializeField] Transform itemListContent;

    private void Awake()
    {
        Instance = this;

        closeButton.onClick.AddListener(Toggle);
        PlayerManager.Inputs.Player.OpenInventory.performed += (_) => Toggle();

        gameObject.SetActive(false);
    }
    
    public void Toggle()
    {
        gameObject.SetActive(!gameObject.activeSelf);

        WipeList();
        foreach (var item in Depot.Inventory.Items.Keys)
            Instantiate(itemPrefab, itemListContent).Build(item);
    }

    void WipeList()
    {
        foreach (Transform child in itemListContent)
            Destroy(child.gameObject);
    }
}
