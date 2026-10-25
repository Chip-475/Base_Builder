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
        if (!Data.botPrefabs.Contains(botPrefab))
        {
            Debug.Log($"No bot of type {botPrefab.name} found.");
            return null;
        }

        return MonoBehaviour.Instantiate(botPrefab, BotSpawnPoint.position, Quaternion.identity).RuntimeObj;
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
