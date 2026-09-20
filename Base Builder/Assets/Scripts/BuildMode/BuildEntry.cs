using UnityEngine;

public class BuildEntry : MonoBehaviour
{
    public InstalledObject buildableObject;
    public void OnClick()
    {
        BuildMode.Instance.SelectedObject = buildableObject;
        //to add:selected object sprite sparkle effect
    }
}
