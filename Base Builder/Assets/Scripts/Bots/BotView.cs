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
        InitializeRuntime();
    }

    public virtual void InitializeRuntime()
    {
        if (RuntimeObj == null)
            RuntimeObj = new Bot(this, data, new() { baseSpawnCoords = Vector3.zero });
    }
    private void Update()
    {
        if (RuntimeObj == null)
            return;

        RuntimeObj.InteractionBounds = new(transform.position, new Vector3(2f, 2f, 0));
        if (powerBar != null)
            powerBar.fillAmount = RuntimeObj.MaxPower <= 0 ? 0 : RuntimeObj.Power / RuntimeObj.MaxPower;
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

    private void OnDestroy()
    {
        StopMoving();
        RuntimeObj?.Dispose();
    }

    async UniTask FollowPath(CancellationToken cancellationToken, CancellationTokenSource source)
    {
        try
        {
            while (RuntimeObj != null && RuntimeObj.Path.Count > 0)
            {
                RuntimeObj.MoveTowardsDestination();
                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
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
