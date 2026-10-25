using UnityEngine;
using UnityEngine.UI;

public class TechTreeEntry : MonoBehaviour
{
    [SerializeField] Tech tech;
    [SerializeField] Image icon;
    [SerializeField] Button button;
    
    private void Start()
    {
        if(tech==null)
        {
            Debug.Log("tech null problema");
            enabled=false;
            return;
        }
        if(icon!=null)icon.sprite=tech.icon;
        button.onClick.AddListener(() => tech.Unlock());
    }
    private void Update()
    {
        button.interactable = tech.IsUnlockable();
    }
}
