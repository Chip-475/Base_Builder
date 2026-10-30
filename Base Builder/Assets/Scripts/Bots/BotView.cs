using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class BotView : MonoBehaviour
{
    public BotData data;
    public Bot RuntimeObj { get; protected set; }

    [Header("Components")]
    public SpriteRenderer spriteRenderer;
    public Animator animator;
    public Image powerBar;

    CancellationTokenSource movementCancellationSource;

    protected virtual void Start()
    {
        RuntimeObj = new Bot(this, data, new() { baseSpawnCoords = Vector3.zero });
    }
    private void Update()
    {
        RuntimeObj.InteractionBounds = new(transform.position, new Vector3(2f, 2f, 0));
        powerBar.fillAmount = RuntimeObj.Power / 100;
    }

    public void StartMoving()
    {
        StopMoving();
        if (RuntimeObj == null || RuntimeObj.Path.Count == 0)
            return;

        CancellationTokenSource source = CancellationTokenSource.CreateLinkedTokenSource(
            this.GetCancellationTokenOnDestroy());
        movementCancellationSource = source;
        FollowPath(source.Token, source).Forget();
    }
    public void StopMoving()
    {
        movementCancellationSource?.Cancel();
    }

    async UniTask FollowPath(CancellationToken cancellationToken, CancellationTokenSource source)
    {
        try
        {
            for (int i = 1; i < RuntimeObj.Path.Count; i++)
            {
                Vector3 target = RuntimeObj.Path[i];

                while (Vector3.Distance(transform.position, target) > 0.01f)
                {
                    transform.position = Vector3.MoveTowards(
                        transform.position,
                        target,
                        data.speed * Time.deltaTime
                    );

                    await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
                }

                transform.position = target;
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            // Cancellation is the normal way to stop visual path following.
        }
        finally
        {
            if (ReferenceEquals(movementCancellationSource, source))
            {
                movementCancellationSource = null;
                source.Dispose();
            }
        }
    }
}
