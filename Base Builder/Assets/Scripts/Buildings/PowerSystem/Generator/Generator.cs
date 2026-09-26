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
}
