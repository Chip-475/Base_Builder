using Unity.VisualScripting;
using UnityEngine;

public class BuildModeButton : MonoBehaviour
{
    [Header("References")]
    public GameObject overlayPanel;

    public void OnClick()
    {
        Player.Instance.buildMode=!Player.Instance.buildMode;
        overlayPanel.SetActive(Player.Instance.buildMode);
    }
}
