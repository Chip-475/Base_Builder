using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class FuelEntry : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] Button button;
    [SerializeField] Image fuelIcon;
    [SerializeField] TMP_Text fuelText; //form "name x fv/s"

    [Header("Runtime Data")]
    public ResourceSO fuel;

    TMP_Text quantityText;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() => GeneratorUI.Instance.selectedEntry = this);
        button.onClick.AddListener(() => GeneratorUI.Instance.BuildInfoPanel(this));
        
        quantityText = GetComponentInChildren<TMP_Text>();
    }

    public void Build(ResourceSO fuel)
    {
        this.fuel = fuel;
        fuelIcon.sprite = fuel.Sprite;
        quantityText.text = fuel.name + " (" + fuel.fuelValue + "/s )";
    }
}
