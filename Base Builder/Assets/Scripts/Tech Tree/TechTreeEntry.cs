using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TechTreeEntry : MonoBehaviour
{
    [SerializeField] TechSO tech;
    [SerializeField] Image icon;
    [SerializeField] Button button;
    
    public TechSO Tech
    {
        get {return tech;}
    }

    private void Start()
    {
        if(tech==null)
        {
            Debug.Log("tech non asseganto");
            enabled=false;
            return;
        }
        if (icon != null)
            icon.sprite = tech.icon;

        button.onClick.AddListener(() => TechTree.UnlockTech(tech));
    }
    private void Update() => button.interactable = TechTree.IsTechUnlockable(tech);
}
