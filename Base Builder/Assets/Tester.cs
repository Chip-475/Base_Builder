using UnityEngine;
using System.Collections.Generic;
using System.Linq;

#pragma warning disable
public class Tester : MonoBehaviour
{
    public static Tester Instance { get; private set; }

    public Bot bot;
    public Vector3Int end;

    public MineralNode mineralNode;

    private void Awake()
    {
        Instance = this;
    }

    [ContextMenu("Move")]
    public void Move()
    {
        bot.MoveTo(end);
    }

    public void GoToMineralNode(MineralNode mineralNode)
    {
        bot.goToMineralNode(mineralNode); 
         
    }
}
