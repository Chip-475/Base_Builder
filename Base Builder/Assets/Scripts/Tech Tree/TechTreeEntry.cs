using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TechTreeEntry : MonoBehaviour
{
    [SerializeField] List<TechTreeEntry> nextEntries = new();
    [Space]
    [SerializeField] TechSO tech;
    [SerializeField] Image icon;
    [SerializeField] Button button;
    [SerializeField] TMP_Text techName;

    private void Start()
    {
        icon ??= GetComponent<Image>();
        button ??= GetComponent<Button>();
        techName ??= GetComponentInChildren<TMP_Text>(true);

        if (tech != null)
        {
            if (icon != null && tech.icon != null)
                icon.sprite = tech.icon;
            if (techName != null)
                techName.text = tech.name;
            if (button != null)
                button.onClick.AddListener(() => TechTree.UnlockTech(tech));
        }

        foreach (var entry in nextEntries)
        {
            Image lineObject = Instantiate(new GameObject()).AddComponent<Image>();
            lineObject.transform.SetParent(transform);

            Vector2 direction = transform.position - entry.transform.position;

            lineObject.rectTransform.anchoredPosition = ((transform.position + entry.transform.position) / 2f) - transform.position;
            lineObject.rectTransform.sizeDelta = new Vector2(direction.magnitude, 15f);
            lineObject.rectTransform.localRotation = Quaternion.Euler(
                0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg
            );

            lineObject.transform.SetParent(transform.parent);
            lineObject.transform.SetSiblingIndex(0);
        }
    }

    private void Update()
    {
        if (button != null && tech != null)
            button.interactable = TechTree.IsTechUnlockable(tech);
    }
}
