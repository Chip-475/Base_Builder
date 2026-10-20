using UnityEngine;
using System.Collections;

public interface IInteractable
{
    public Vector3 Position { get; }
    public Cell[] InteractionCells { get; }
    public void Interact();
    public bool CanInteractWith();
}

public interface IWorker
{
    public IWorkplace Workplace { get; set; }
}
public interface IWorkplace
{
    public IEnumerator Work(BotManager.WorkRequest request);
    public bool CanWork(BotManager.WorkRequest request);
    public bool HasRequiredItems(BotManager.WorkRequest request);
}