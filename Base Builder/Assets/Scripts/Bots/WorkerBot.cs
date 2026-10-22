using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class WorkerBot : Bot
{
    public WorkerBot(BotView view, BotData data, BotConfig config) : base(view, data, config)
    {
    }

    // IWorker
    public override IEnumerator Pipeline()
    {
        throw new System.NotImplementedException();
    }
}
