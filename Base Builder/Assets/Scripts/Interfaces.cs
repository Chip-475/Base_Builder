using UnityEngine;

public interface IInteractable
{
    public Vector3 Position { get; }
    public Cell[] InteractionCells { get; }
    public void Interact();
    public bool CanInteractWith();
}
public interface IWorkplace
{
    public RecipeSO Work(RecipeSO recipe, Bot bot);
    public bool CanWork(RecipeSO recipe, Bot bot);
    public bool HasRequiredItems(RecipeSO recipe, Bot bot);
}