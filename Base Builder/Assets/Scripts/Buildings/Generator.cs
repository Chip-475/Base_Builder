using System;
using UnityEngine;

public class Generator : Building
{
    [Serializable]
    public class GeneratorConfig : Config
    {
        // fill out
    }

    public bool Production { get; private set; }

    public Generator(Generator_View obj, GeneratorConfig config) : base(obj, config)
    {

    }
}
