using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;

public class Spaceship : Building
{
    public new SpaceshipData Data => base.Data as SpaceshipData;
    public new SpaceshipView SceneObj => base.SceneObj as SpaceshipView;
    public SpaceshipUI UI { get; private set; }

    public Transform BotSpawnPoint { get; private set; }
    public float PowerGeneration => Data.powerGeneration;

    public Spaceship(SpaceshipData data, SpaceshipView sceneObj, Transform botSpawnPoint, SpaceshipUI ui) : base(data, sceneObj)
    {
        BotSpawnPoint = botSpawnPoint;
        SpaceshipUI.spaceship = this;
        UI = ui;
    }
    public override void Destroy()
    {
        throw new NotImplementedException();
    }

    public Bot BuildBot(BotView botPrefab)
    {
        if (botPrefab == null || botPrefab.data == null || BotSpawnPoint == null)
        {
            Debug.LogWarning("Cannot build a bot: its prefab or spawn point is missing.");
            return null;
        }

        if (Depot.Inventory == null || !Depot.Inventory.CanRemoveItems(botPrefab.data.cost))
            return null;

        Depot.Inventory.RemoveItems(botPrefab.data.cost);
        BotView view = MonoBehaviour.Instantiate(botPrefab, BotSpawnPoint.position, Quaternion.identity);
        view.InitializeRuntime();
        return view.RuntimeObj;
    }

    // Work
    public override Dictionary<ResourceSO, int> GetWorkOutputs() => null;

    // Interact

    public void OnClick()
    {
        if (Helpers.IsMouseOverUI())
            return;

        UI.Open();
    }
}
