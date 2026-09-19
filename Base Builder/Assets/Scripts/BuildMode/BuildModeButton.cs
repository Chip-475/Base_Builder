using Unity.VisualScripting;
using UnityEngine;

public class BuildModeButton : MonoBehaviour
{
    [Header("References")]
    public GameObject overlayPanel;
    public void OnClick()
    {
        Player.instance.buildMode=!Player.instance.buildMode;
        overlayPanel.SetActive(Player.instance.buildMode);
    }
}
