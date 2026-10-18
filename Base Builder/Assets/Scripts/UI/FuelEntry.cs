using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(Button))]
public class FuelEntry : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] Button button;
    [SerializeField] Image fuel;
    [SerializeField] TMP_Text fuelText; //form "name x fv/s"

    [Header("Runtime Data")]
    public ResourceSO recipe;

    TMP_Text quantityText;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() => GeneratorUI.instance.selectedFuelEntry = this);
        button.onClick.AddListener(() => GeneratorUI.instance.BuildInfoPanel(this));

        quantityText = GetComponentInChildren<TMP_Text>();
    }

    public void Build(ResourceSO recipe)
    {
        this.recipe = recipe;
        Sprite currentEntry = recipe.Sprite;
        quantityText.text = recipe.name + " (" + recipe.fv + "/s )";
    }
}
