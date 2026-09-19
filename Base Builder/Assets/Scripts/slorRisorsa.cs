using UnityEngine;
using UnityEngine.UI;
public class slorRisorsa : MonoBehaviour
{
    public Image icona;
    public Text nomeTesto;
    public Text quantiTesto;

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
