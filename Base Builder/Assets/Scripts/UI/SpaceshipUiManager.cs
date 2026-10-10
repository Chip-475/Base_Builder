using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SpaceshipUiManager : MonoBehaviour
{
    public static Spaceship spaceship;
    public bool IsActive => gameObject.activeSelf;

    readonly Stack<GameObject> history = new();
    GameObject lastOpened;

    [Header("Buttons")]
    [SerializeField] Button botMenuButton;
    [SerializeField] Button spaceshipMenuButton;
    [SerializeField] Button inventoryMenuButton;
    [SerializeField] Button techTreeMenuButton;

    [Header("Menus")]
    [SerializeField] GameObject botMenu;
    [SerializeField] GameObject spaceshipMenu;
    [SerializeField] GameObject inventoryMenu;
    [SerializeField] GameObject techTreeMenu;

    private void Awake()
    {
        botMenuButton.onClick.AddListener(() => OpenMenu(botMenu));
        spaceshipMenuButton.onClick.AddListener(() => OpenMenu(spaceshipMenu));
        inventoryMenuButton.onClick.AddListener(() => OpenMenu(inventoryMenu));
        techTreeMenuButton.onClick.AddListener(() => OpenMenu(techTreeMenu));
    }

    public void Toggle()
    {
        if (IsActive) Close();
        else Open();
    }

    void Open()
    {
        gameObject.SetActive(true);

        if (lastOpened != null)
            OpenMenu(lastOpened);
    }
    void Close()
    {
        gameObject.SetActive(false);
    }
    void OpenMenu(GameObject menu)
    {
        if (!IsActive)
            return;
        history.Pop().SetActive(false);
        history.Push(menu);

        menu.SetActive(true);
        lastOpened = menu;
    }
}
