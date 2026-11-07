using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemEntry : MonoBehaviour
{
    [SerializeField] Button button;
    [SerializeField] Image Icon;
    [SerializeField] TMP_Text Amount;

    public bool Build(ResourceSO resource)
    {
        button.onClick.AddListener(() =>
        {
            if (InventoryMenu.Instance.discardMode)
            {
                Depot.Inventory.Items.Remove(resource);
                Destroy(gameObject);
            }
        });

        if (!Depot.Inventory.Items.ContainsKey(resource))
            return false;

        Icon.sprite = resource.Sprite;
        Amount.text = Depot.Inventory.Items[resource].ToString();
        return true;
    }
}
