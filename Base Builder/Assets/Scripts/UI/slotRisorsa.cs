using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class slotRisorsa : MonoBehaviour
{
    public Image icona;
    public TMP_Text nomeTesto;
    public TMP_Text quantiTesto;

    public void imposta(ResourceSO ris)
    {
        if (icona != null) icona.sprite = ris.r_sprite;
        if (nomeTesto != null) nomeTesto.text = ris.r_name;
    }
    public void impostaQuant(ResourceSO ris,int quant)
    {
        imposta(ris);
        if (quantiTesto.text != null) quantiTesto.text="x"+quant;
    }
}
