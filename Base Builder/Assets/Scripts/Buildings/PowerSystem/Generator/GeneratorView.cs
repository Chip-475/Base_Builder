using UnityEngine;

public class GeneratorView : BuildingView
{
    public new GeneratorData Data => base.Data as GeneratorData;
    private Generator generator;

    private void Start()
    {
        generator = new Generator(Data, this);
    }
    private void OnDestroy()
    {
        if (generator == null) return;
        PowerManager.powerGeneratorDB.Remove(generator.id);
        if (PowerManager.instance != null) PowerManager.instance.UnregisterBuilding();
    }
}
