using UnityEngine;
using System;
using System.Collections.Generic;

public class Machine : Building
{
    [Serializable]
    public class MachineConfig : Config
    {
        [Space]
        public RecipeSO[] recipes;
    }

    public RecipeSO[] Recipes { get; private set; }

    public Machine(Machine_View obj, MachineConfig config) : base(obj, config)
    {
        Recipes = config.recipes;
    }
}
