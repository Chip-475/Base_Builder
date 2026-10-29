using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemEntry : MonoBehaviour
{
    [SerializeField] Image Icon;
    [SerializeField] TMP_Text Amount;

    public bool Build(ResourceSO resource)
    {
        if (!Depot.Inventory.Items.ContainsKey(resource))
            return false;

        Icon.sprite = resource.Sprite;
        Amount.text = Depot.Inventory.Items[resource].ToString();
        return true;
    }
}
