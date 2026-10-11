using System;
using System.Collections.Generic;
using UnityEngine;

public class SpaceshipGenerator : Generator
{
    public new SpaceshipGeneratorData Data => base.Data as SpaceshipGeneratorData;
    public new SpaceshipGeneratorView SceneObj => base. as SpaceshipGeneratorView;

    public SpaceshipGenerator(SpaceshipGeneratorData data, GeneratorView sceneObj) : base(data, sceneObj)
    {
        id = Guid.NewGuid().ToString();
        PowerManager.powerGeneratorDB[id] = this;
        PowerManager.instance.RegisterBuilding();
    }


}
