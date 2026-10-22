using UnityEngine;
using System.Collections.Generic;
using System.Linq;

#pragma warning disable
public class Tester : MonoBehaviour
{
    public static Tester Instance { get; private set; }

    public MinerBotView bot;
    public MineralNodeView node;
    public MineralCrateView crate;
    
    public Vector3 pos;

    private void Awake()
    {
        Instance = this;
    }
    
    [ContextMenu("Work Test")]
    public void WorkTest()
    {
        MinerBot bot = this.bot.RuntimeObj;
        bot.Node = node.RuntimeObj as MineralNode;
        bot.Crate = crate.RuntimeObj as MineralCrate;

        bot.StartWorking();
    }
}
