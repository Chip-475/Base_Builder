using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using TMPro;
using Unity.VisualScripting;

public class SpaceshipUiManager : MonoBehaviour
{
    public static Spaceship spaceship;
    public bool IsActive => gameObject.activeSelf;

    readonly Stack<GameObject> history = new();
    GameObject lastOpened;

    [Header("Buttons")]
    [SerializeField] Button closeButton;
    [Space]
    [SerializeField] Button botMenuButton;
    [SerializeField] Button spaceshipMenuButton;
    [SerializeField] Button inventoryMenuButton;
    [SerializeField] Button techTreeMenuButton;

    [Header("Menus")]
    [SerializeField] GameObject botMenuPanel;
    [SerializeField] BotMenu botMenu = new();
    [Space]
    [SerializeField] GameObject spaceshipMenuPanel;
    [SerializeField] SpaceshipMenu spaceshipMenu;
    [Space]
    [SerializeField] GameObject inventoryMenuPanel;
    [SerializeField] InventoryMenu inventoryMenu;
    [Space]
    [SerializeField] GameObject techTreeMenuPanel;
    [SerializeField] TechTreeMenu techTreeMenu;

    private void Awake()
    {
        closeButton.onClick.AddListener(Close);

        botMenuButton.onClick.AddListener(() => ToggleMenu(botMenuPanel));
        spaceshipMenuButton.onClick.AddListener(() => ToggleMenu(spaceshipMenuPanel));
        inventoryMenuButton.onClick.AddListener(() => ToggleMenu(inventoryMenuPanel));
        techTreeMenuButton.onClick.AddListener(() => ToggleMenu(techTreeMenuPanel));

        botMenu.Initialize();
    }

    public void Open()
    {
        if (Helpers.IsMouseOverUI())
            return;

        gameObject.SetActive(true);
        PlayerManager.DisableAll();

        if (lastOpened != null)
            ToggleMenu(lastOpened);
        else
            ToggleMenu(botMenuPanel);
    }
    public void Close()
    {
        gameObject.SetActive(false);
        PlayerManager.PlayerMode();
    }
    void ToggleMenu(GameObject menu)
    {
        if (!IsActive)
            return;
        if(history.Count != 0)
            history.Pop().SetActive(false);
        history.Push(menu);

        menu.SetActive(true);
        lastOpened = menu;
    }

    [Serializable]
    public class BotMenu
    {
        [Header("Setup")]
        [SerializeField] GameObject botListContent;
        [Space]
        [SerializeField] Image botImage;
        [SerializeField] TMP_Text botName;
        [SerializeField] Button previous;
        [SerializeField] Button next;
        [SerializeField] Button build;

        [Header("Config")]
        [SerializeField] List<BotView> botPrefabs;

        int currentBotIndex = 0;

        public void Initialize()
        {
            SwitchBot(0);

            // Actions
            previous.onClick.AddListener(() => SwitchBot(-1));
            next.onClick.AddListener(() => SwitchBot(1));
        }

        void SwitchBot(int amount)
        {
            int nextIndex = currentBotIndex + amount;
            if (nextIndex < 0) nextIndex = botPrefabs.Count - 1;
            else if (nextIndex > botPrefabs.Count - 1) nextIndex = 0;

            botImage.sprite = botPrefabs[nextIndex].data.sprite;
            botName.text = botPrefabs[nextIndex].data.name;
            currentBotIndex = nextIndex;
        }
    }
    [Serializable]
    public class SpaceshipMenu
    {

    }
    [Serializable]
    public class InventoryMenu
    {

    }
    [Serializable]
    public class TechTreeMenu
    {

    }
}
