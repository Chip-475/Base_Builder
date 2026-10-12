using UnityEngine;

public interface IInteractable
{
    public Vector3 Position { get; }
    public Cell[] InteractionCells { get; }
    bool CanInteractWith();
    void Interact();
}