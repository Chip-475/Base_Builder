using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface IInteractable
{
    public Vector3 Position { get; }
    public Cell[] InteractionCells { get; }
    public void Interact();
    public bool CanInteractWith();
}

// Work needs worker, workplace and what it needs to do
// What it needs to do can be expressed as:
// - Dictionary of resource to int amount as input
// - Dictionary of resource to int amount as output
public interface IWorker
{
    public Bot GetBot();
    public IEnumerator Pipeline();
    public void Fail(string reason);

    public bool IsWorking { get; }
    public Coroutine WorkRoutine { get; }
}
public interface IWorkplace
{
    public Building GetBuilding();
    public Dictionary<ResourceSO, int> GetWorkOutputs();
}