using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Text;

public class machineAutoUI : MonoBehaviour
{
    public RecipeSO recipe;
    public Canvas canvas;
    public TMP_Text info;
    public TMP_Text bar;
    public Button btnInput;   // bottoni che poi vanno sostituiti che portano e prendono le ris i bot
    public Button btnOutput;
    MachineView view;

    void Awake()
    {
        view=GetComponent<MachineView>();
        if (canvas != null && canvas.worldCamera == null)
            canvas.worldCamera = Camera.main;
        if (btnInput != null)
            btnInput.onClick.AddListener(aggI);
        if (btnOutput != null)
            btnOutput.onClick.AddListener(prenOut);
    }

    void LateUpdate()
    {
        if (view == null || view.RuntimeObj == null)
            return;

        if (Camera.main != null && canvas != null)
            canvas.transform.rotation = Camera.main.transform.rotation;
        var m = view.RuntimeObj;
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"{m.Type} - {(m.lavoro?"lavora":"fermo")}");
        if (m.aspeEne) sb.AppendLine("attesa di energia");
        sb.Append("input ");
        foreach(var kv in m.input)sb.Append($"{kv.Key.name} x{kv.Value} ");
        sb.AppendLine();
        sb.AppendLine("out ");
        foreach (var kv in m.output) sb.Append($"{kv.Key.name} x{kv.Value}");
        if (info != null)
            info.text = sb.ToString();
        if (bar != null)
            bar.text = m.progresso.ToString("P0");
    }

    private void aggI()
    {
        var m=view.RuntimeObj;
        if (m == null || recipe == null || m.Data == null || m.Data.usableRecipes == null ||
            !m.Data.usableRecipes.Contains(recipe)) return;
        m.SelectRecipe(recipe);
        m.RequestInputsFromDepot(recipe.inputs);
    }
    private void prenOut()
    {
        var m = view.RuntimeObj;
        if (m == null) return;
        m.RequestOutputDelivery();
    }    
}
