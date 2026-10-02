using UnityEngine;

public class GeneratorView : BuildingView
{
    public new GeneratorData Data => base.Data as GeneratorData;
    public override void Init()
    {

    }
    private void Start()
    {
        new Generator(Data, this);
    }
}
