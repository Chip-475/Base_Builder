using UnityEngine;
using System.Collections.Generic;
using System.Linq;

#pragma warning disable
public class Tester : MonoBehaviour
{
    public static Tester Instance { get; private set; }

    public BuildingView building;
    public BotView bot;
    public Vector3 pos;

    private void Awake()
    {
        Instance = this;
    }
    
    [ContextMenu("Work Test")]
    public void WorkTest()
    {
        var toWorkAt = building as MachineView;
        toWorkAt.Obj.AssignBot(bot.RuntimeObj);
    }

    [ContextMenu("Move Test")]
    public void MoveTest()
    {
        bot.SetDestination(pos.ToVector3Int());
        bot.StartMoving();
    }
}
