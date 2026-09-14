using UnityEngine;
using UnityEngine.EventSystems;

public class BotBehaviour : IPointerDownHandler
{ 
    //idk how to link this script to a bot instance,so it can be moved to a proper place later
    public void OnPointerDown(PointerEventData eventData)
    {
        
    }
    public void WalkTo(Vector2Int cellCoords)
    {
        // Implement A* to move the bot to the target
    }
    public void ModifyEnergy(int amount)
    {
        // Implement energy modification logic
    }
}
