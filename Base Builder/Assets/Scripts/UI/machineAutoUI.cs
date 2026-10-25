using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Text;
using Cysharp.Threading.Tasks;
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
        if(canvas!=null&&canvas.worldCamera==null)canvas.worldCamera=Camera.main;
        btnInput.onClick.AddListener(aggI);
        btnOutput.onClick.AddListener(prenOut);
    }

    void LateUpdate()
    {
        if (Camera.main != null) canvas.transform.rotation = Camera.main.transform.rotation;
        var m = view.RuntimeObj;
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"{m.Type} - {(m.lavoro?"lavora":"fermo")}");
        if (m.aspeEne) sb.AppendLine("attesa di energia");
        sb.Append("input ");
        foreach(var kv in m.input)sb.Append($"{kv.Key.name} x{kv.Value} ");
        sb.AppendLine();
        sb.AppendLine("out ");
        foreach (var kv in m.output) sb.Append($"{kv.Key.name} x{kv.Value}");
        info.text=sb.ToString();
        bar.text = ""+m.progresso;
    }

    private void aggI()
    {
        var m=view.RuntimeObj;
        if (m == null || recipe == null) return;
        foreach (var kv in recipe.inputs) m.aggIn(kv.Key, kv.Value);
    }
    private void prenOut()
    {
        var m = view.RuntimeObj;
        if (m == null) return;
        foreach(var k in new List<ResourceSO>(m.output.Keys))
        {
            m.prendiOut(k,m.output[k]);
        }
    }    
}
