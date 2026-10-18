using UnityEngine;
using UnityEngine.UI;

public class TechTreeEntry : MonoBehaviour
{
    [SerializeField] Tech tech;
    [SerializeField] Image icon;
    [SerializeField] Button button;

    private void Start()
    {
        button.onClick.AddListener(() => tech.Unlock());
    }
    private void Update()
    {
        button.interactable = tech.IsUnlockable();
    }
}
