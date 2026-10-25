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
    public Slider bar;
    public Button btnInput;   // bottoni che poi vanno sostituiti che portano e prendono le ris i bot
    public Button btnOutput;
    MachineView view;

    void Awake()
    {
        view=GetComponent<MachineView>();
        if(canvas!=null&&canvas.worldCamera==null)canvas.worldCamera=Camera.main;
        btnInput.onClick.AddListener(aggIn);
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
        foreach
    }
}
