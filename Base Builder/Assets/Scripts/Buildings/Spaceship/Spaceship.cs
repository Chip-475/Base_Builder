using UnityEditor;
using UnityEngine;
using System;

public class Spaceship : Building
{
    public new SpaceshipData Data => base.Data as SpaceshipData;

    public Transform BotSpawnPoint { get; private set; }
    public float PowerGeneration => Data.powerGeneration;
    public SpaceshipUiManager UI { get; private set; }

    public Spaceship(SpaceshipData data, SpaceshipView sceneObj, Transform botSpawnPoint, SpaceshipUiManager ui) : base(data, sceneObj)
    {
        BotSpawnPoint = botSpawnPoint;
        SpaceshipUiManager.spaceship = this;
        UI = ui;
    }
    public override void Destroy()
    {
        throw new NotImplementedException();
    }

    public Bot BuildBot(BotType type)
    {
        BotView bot = Data.botPrefabs[type];
        MonoBehaviour.Instantiate(bot, BotSpawnPoint.position, Quaternion.identity);

        return bot.RuntimeObj;
    }

    public void OnClick()
    {
        UI.Open();
    }
}
