using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks.Triggers;

public class Player: MonoBehaviour
{
    public static Player instance;
    public bool botClicked;
    public bool buildMode;
    public InputSystem_Actions inputActions;

    void Awake()
    {
        inputActions = new();
        instance = this;
        inputActions.Player.Enable();
        inputActions.Player.PlaceBuilding.performed+= (diocane) => Debug.Log(WorldManager.World.GetCellAt(Camera.main.ScreenToWorldPoint(Input.mousePosition).ToVector3Int()).canPlaceOn); ;
    }
    /*
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            controllaClick();
        }
    }
    private void controllaClick()
    {
        Vector2 pos=Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero);
        if (hit.collider != null)
        {
            MineraleNode mine = hit.collider.GetComponent<MineraleNode>();
            if (mine != null)
            {
                botClicked = true;
                mine.colpisci();
            }
            else botClicked = false;
        }
        else botClicked = false;
    }*/

}
