using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using TMPro;
using System.Linq;

public class SpaceshipUI : MonoBehaviour
{
    public static Spaceship spaceship;
    public bool IsActive => gameObject.activeSelf;

    readonly Stack<GameObject> history = new();
    GameObject lastOpened;

    [SerializeField] GameObject panel;
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
    [SerializeField] SpaceshipMenu spaceshipMenu = new();
    [Space]
    [SerializeField] GameObject inventoryMenuPanel;
    [SerializeField] InventoryMenu inventoryMenu = new();
    [Space]
    [SerializeField] GameObject techTreeMenuPanel;
    [SerializeField] TechTreeMenu techTreeMenu = new();

    private void Awake()
    {
        closeButton.onClick.AddListener(Close);

        botMenuButton.onClick.AddListener(() => ToggleMenu(botMenuPanel));
        spaceshipMenuButton.onClick.AddListener(() => ToggleMenu(spaceshipMenuPanel));
        inventoryMenuButton.onClick.AddListener(() => ToggleMenu(inventoryMenuPanel));
        techTreeMenuButton.onClick.AddListener(() => ToggleMenu(techTreeMenuPanel));

        botMenu.Initialize();
    }
    private void Update()
    {
        botMenu.Update();
    }

    public void Open()
    {
        if (Helpers.IsMouseOverUI())
            return;

        panel.SetActive(true);
        PlayerManager.DisableAll();

        if (lastOpened != null)
            ToggleMenu(lastOpened);
        else
            ToggleMenu(botMenuPanel);
    }
    public void Close()
    {
        panel.SetActive(false);
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
        [SerializeField] Transform botSpawnpoint;
        [Space]
        [SerializeField] Image botImage;
        [SerializeField] TMP_Text botName;
        [SerializeField] Button previous;
        [SerializeField] Button next;
        [SerializeField] Button buildButton;

        [Header("Config")]
        [SerializeField] List<BotView> botPrefabs;

        int currentBotIndex = 0;

        public void Initialize()
        {
            SwitchBot(0);

            // Actions
            if (previous != null)
                previous.onClick.AddListener(() => SwitchBot(-1));
            if (next != null)
                next.onClick.AddListener(() => SwitchBot(1));
            if (buildButton != null)
                buildButton.onClick.AddListener(BuildBot);
        }
        public void Update()
        {
            if (buildButton == null)
                return;

            BotView selected = GetSelectedBot();
            buildButton.interactable = selected != null && selected.data != null
                && Depot.Inventory != null
                && Depot.Inventory.CanRemoveItems(selected.data.cost)
                && spaceship != null;
        }

        void SwitchBot(int amount)
        {
            if (botPrefabs == null || botPrefabs.Count == 0)
            {
                currentBotIndex = 0;
                if (buildButton != null) buildButton.interactable = false;
                if (botName != null) botName.text = "No bots available";
                if (botImage != null) botImage.sprite = null;
                return;
            }

            int nextIndex = (currentBotIndex + amount) % botPrefabs.Count;
            if (nextIndex < 0)
                nextIndex += botPrefabs.Count;

            BotView selected = botPrefabs[nextIndex];
            if (selected == null || selected.data == null)
                return;

            if (botImage != null) botImage.sprite = selected.data.sprite;
            if (botName != null) botName.text = selected.data.name;
            currentBotIndex = nextIndex;
        }

        BotView GetSelectedBot()
        {
            return botPrefabs != null && currentBotIndex >= 0 && currentBotIndex < botPrefabs.Count
                ? botPrefabs[currentBotIndex]
                : null;
        }

        void BuildBot()
        {
            BotView selected = GetSelectedBot();
            if (selected == null || spaceship == null)
                return;

            spaceship.BuildBot(selected);
        }
    }
    [Serializable]
    public class SpaceshipMenu
    {
        [Header("Setup")]
        [SerializeField] Button navComponent;
        [SerializeField] Dictionary<ResourceSO, int> navCost;
        [Space]
        [SerializeField] Button coreComponent;
        [SerializeField] Dictionary<ResourceSO, int> coreCost;
        
        public void Initialize()
        {
            //navComponent.onClick.AddListener(Unlock);
            //coreComponent.onClick.AddListener(Unlock);

            // load progress
        }
        public void Update()
        {
            navComponent.interactable = Depot.Inventory.CanRemoveItems(navCost);
            coreComponent.interactable = Depot.Inventory.CanRemoveItems(coreCost);
        }
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
