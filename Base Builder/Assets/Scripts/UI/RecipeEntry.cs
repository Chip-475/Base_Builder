using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class RecipeEntry : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] Button button;
    [SerializeField] Button addButton;
    [SerializeField] Button subtractButton;
    [Space]
    [SerializeField] Image outputIcon;
    [SerializeField] List<Image> inputsIcons;

    [Header("Runtime Data")]
    public RecipeSO recipe;
    public int Amount { get; set; } = 1;

    readonly Dictionary<Image, TMP_Text> quantityText = new Dictionary<Image, TMP_Text>();
    bool inizializzato;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        if (inizializzato) return;
        button = GetComponent<Button>();
        if (addButton == null || subtractButton == null || outputIcon == null)
        {
            Debug.Log("dio monty python");
            return;
        }
        button.onClick.AddListener(() => MachineUI.Instance.selectedRecipeEntry = this);
        button.onClick.AddListener(() => MachineUI.Instance.BuildInfoPanel(this));

        addButton.onClick.AddListener(() => ModifyAmount(1));
        subtractButton.onClick.AddListener(() => ModifyAmount(-1));

        quantityText[outputIcon] = outputIcon.GetComponentInChildren<TMP_Text>();
        /*foreach (var item in inputsIcons)
            quantityText[item] = item.GetComponentInChildren<TMP_Text>();*/
        inizializzato = true;
    }

    TMP_Text textDi(Image icon)
    {
        if (icon == null) return null;
        if(!quantityText.TryGetValue(icon, out var testo))
        {
            testo=icon.GetComponentInChildren<TMP_Text>(true);
            quantityText[icon]=testo;
        }
        return testo;
    }

    void impostaQuant(Image icona,int val)
    {
        TMP_Text text=textDi(icona);
        if(text!=null)text.text=val.ToString();
    }

    private void Update()
    {
        if (!inizializzato || recipe == null || Depot.Inventory == null) return;
        int amount = Amount + 1;
        bool plus = true;
        foreach(KeyValuePair<ResourceSO,int>item in recipe.inputs)
        {
            if (!Depot.Inventory.CanRemoveItem(item.Key, item.Value * amount))plus=false;
        }
        foreach(KeyValuePair<ResourceSO,int>item in recipe.outputs)
        {
            if (!Depot.Inventory.CanAddItem(item.Key, item.Value / amount)) plus = false;
        }
        addButton.interactable = plus;
        subtractButton.interactable = true;
        /*
        foreach (var item in recipe.inputs)
            if (!Depot.Inventory.CanRemoveItem(item.Key, item.Value * Amount))
                addButton.interactable = false;

        foreach (var item in recipe.outputs)
            if (!Depot.Inventory.CanAddItem(item.Key, item.Value * Amount))
                addButton.interactable = false;
        */
    }

    public void Build(RecipeSO recipe)
    {
        Init();
        if (!inizializzato) return;
        if(recipe==null)
        {
            Debug.Log("ric nulla");
            return;
        }
        if (recipe.inputs.Count > inputsIcons.Count || recipe.outputs.Count > 1)
        {
            Debug.Log("ric invalida");
            return;
        }
        this.recipe = recipe;
        foreach(KeyValuePair<ResourceSO,int> prodotto in recipe.outputs)
        {
            outputIcon.sprite = prodotto.Key.Sprite;
            impostaQuant(outputIcon, prodotto.Value * Amount);
            break;
        }
        int indice = 0;
        //var outputs = recipe.outputs.ToArray();
        //var inputs = recipe.inputs.ToArray();
        //quantityText[outputIcon].text = (outputs[0].Value * Amount).ToString();
        /*for (int i = 0; i < 3; i++)
        {
            if (i >= recipe.inputs.Count)
            {
                inputsIcons[i].gameObject.SetActive(false);
                continue;
            }

            inputsIcons[i].gameObject.SetActive(true);
            inputsIcons[i].sprite = inputs[i].Key.Sprite;
            quantityText[inputsIcons[i]].text = (inputs[i].Value * Amount).ToString();
        }*/
        foreach (KeyValuePair<ResourceSO, int> ingrediente in recipe.inputs)
        {
            Image icona = inputsIcons[indice];
            icona.gameObject.SetActive(true);
            icona.sprite = ingrediente.Key.Sprite;
            impostaQuant(icona, ingrediente.Value * Amount);
            indice++;
        }
        for (int i = indice; i < inputsIcons.Count; i++)
        {
            inputsIcons[i].gameObject.SetActive(false);
        }
    }

    void ModifyAmount(int amount)
    {
        if (this.Amount + amount <= 0)
            return;

        foreach (var item in recipe.inputs)
            if (!Depot.Inventory.CanRemoveItem(item.Key, item.Value * (this.Amount + amount)))
                return;

        foreach (var item in recipe.outputs)
            if (!Depot.Inventory.CanAddItem(item.Key, item.Value * (this.Amount + amount)))
                return;

        this.Amount += amount;
        Build(recipe);
    }
}