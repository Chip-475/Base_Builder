using UnityEngine;
using System;

public class MineralNode : Building
{
    [Serializable]
    public class MineralNodeConfig : Config
    {
        [Space]
        public ResourceSO resource;
        public int baseAmount;
        public NodePurity purity;
    }

    public ResourceSO Resource { get; private set; }
    public int BaseAmount { get; private set; }
    public NodePurity Purity { get; private set; }

    public MineralNode(MineralNode_View obj, MineralNodeConfig config) : base(obj, config)
    {
        Resource = config.resource;
        BaseAmount = config.baseAmount;
        Purity = config.purity;
    }

}
public enum NodePurity
{
    Low = 1,
    Medium = 2,
    High = 3
}
