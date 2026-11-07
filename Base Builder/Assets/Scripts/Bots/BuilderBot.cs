using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class BuilderBot : Bot
{
    public ConstructionSite CurrentSite { get; private set; }

    public BuilderBot(BotView view, BotData data, BotConfig config) : base(view, data, config)
    {
        StartWorking();
    }

    public override async UniTask Pipeline(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            if (CurrentSite == null || !CurrentSite.IsPending)
            {
                CurrentSite = ConstructionSite.FindClosestAvailable(Coords, this);

                if (CurrentSite == null)
                {
                    await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
                    continue;
                }
            }

            if (!MoveTo(CurrentSite.Coords))
            {
                CurrentSite.Release(this);
                CurrentSite = null;
                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
                continue;
            }

            if ((Coords - CurrentSite.transform.position).sqrMagnitude > 4f)
            {
                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
                continue;
            }

            float constructionTime = Mathf.Max(0.1f, CurrentSite.Data.constructionTime);
            await UniTask.Delay(
                constructionTime.ToMilliseconds(),
                cancellationToken: cancellationToken);

            if (CurrentSite != null && CurrentSite.IsPending)
                CurrentSite.Complete(this);

            CurrentSite = null;
        }
    }
}
