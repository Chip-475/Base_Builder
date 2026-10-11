using UnityEngine;

public class SpaceshipGeneratorView : BuildingView
{
    public new SpaceshipGeneratorData Data => base.Data as SpaceshipGeneratorData;
    private SpaceshipGenerator generator;

    private void Start()
    {
        generator = new SpaceshipGenerator(Data, this);
    }
    private void OnDestroy()
    {
        if (generator == null) return;
        PowerManager.powerGeneratorDB.Remove(generator.id);
        if (PowerManager.instance != null) PowerManager.instance.UnregisterBuilding();
    }
}
