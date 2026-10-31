using UnityEngine;
using UnityEngine.UI;

public class TechTreeEntry : MonoBehaviour
{
    [SerializeField] TechSO tech;
    [SerializeField] Image icon;
    [SerializeField] Button button;
    
    private void Start()
    {
        if (icon != null)
            icon.sprite = tech.icon;

        button.onClick.AddListener(() => TechTree.UnlockTech(tech));
    }
    private void Update() => button.interactable = TechTree.IsTechUnlockable(tech);
}
