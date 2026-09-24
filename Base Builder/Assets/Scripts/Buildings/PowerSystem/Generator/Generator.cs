using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class Generator : Building
{
    public new GeneratorData Data => base.Data as GeneratorData;
    public new GeneratorView SceneObj => base.SceneObj as GeneratorView;

    public List<PowerPole> ConnectedPoles;

    public float Power;

    public Generator(GeneratorData data, GeneratorView sceneObj) : base(data, sceneObj)
    {

    }

    public void Fuel(ResourceSO fuel, int quantity)
    {
        if (!Data.allowedFuels.Contains(fuel)) return;
        Burn(fuel, quantity);
    }
    async UniTask Burn(ResourceSO fuel,int quantity)
    {

        await UniTask.Delay(0);
    }
}
