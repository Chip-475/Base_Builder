using UnityEditor;
using UnityEngine;
using System;

public class Spaceship : Building
{
    public new SpaceshipData Data => base.Data as SpaceshipData;
    public new SpaceshipView SceneObj => base.SceneObj as SpaceshipView;
    
    public static Spaceship instance;
    public Transform BotSpawnPoint { get; private set; }
    public float PowerGeneration => Data.powerGeneration;
    public SpaceshipUiManager UI { get; private set; }

    public Spaceship(SpaceshipData data, SpaceshipView sceneObj, Transform botSpawnPoint, SpaceshipUiManager ui) : base(data, sceneObj)
    {
        instance = this;
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
        if(!Data.botPrefabs.TryGetValue(type,out BotView prefab)||prefab==null)
        {
            Debug.Log("nessun prefab");
            return null;
        }
        //BotView bot = Data.botPrefabs[type];
        //MonoBehaviour.Instantiate(bot, BotSpawnPoint.position, Quaternion.identity);
        BotView istanza = UnityEngine.Object.Instantiate(prefab, BotSpawnPoint.position, Quaternion.identity);
        return istanza.RuntimeObj;
    }

    public void OnClick()
    {
        if (Helpers.IsMouseOverUI())
            return;

        UI.Open();
    }
}
