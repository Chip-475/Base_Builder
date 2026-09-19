using UnityEngine;

public class BuildEntry : MonoBehaviour
{
    public InstalledObject buildableObject;
    public void OnClick()
    {
        BuildMode.instance.selectedObject = buildableObject;
        //to add:selected object sprite sparkle effect
    }
}
