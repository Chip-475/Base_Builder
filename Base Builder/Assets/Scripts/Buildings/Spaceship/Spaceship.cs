using UnityEditor;
using UnityEngine;
using System;

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

    public Bot BuildBot(BotType type)
    {
        if (!Data.botPrefabs.ContainsKey(type))
        {
            Debug.Log($"No bot of type {type} found.");
            return null;
        }

        return MonoBehaviour.Instantiate(Data.botPrefabs[type], BotSpawnPoint.position, Quaternion.identity).RuntimeObj;
    }

    public void OnClick()
    {
        if (Helpers.IsMouseOverUI())
            return;

        UI.Open();
    }
}
