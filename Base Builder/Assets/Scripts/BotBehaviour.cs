using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class BotBehaviour:IPointerDownHandler
{ 
    //idk how to link this script to a bot instance,so it can be moved to a proper place later
    public void OnPointerDown(PointerEventData eventData)
    {
        if(playerState.instance.botClicked)
        {
            playerState.instance.botClicked = false;
            Debug.Log("Assign Mode Unactive");
        }
        else
        {
            playerState.instance.botClicked = true;
            Debug.Log("Assign Mode Active");
        }
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
