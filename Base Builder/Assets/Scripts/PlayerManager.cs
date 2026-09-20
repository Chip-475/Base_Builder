using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance {  get; private set; }
    public static PlayerInputs Inputs { get; private set; }

    void Awake()
    {
        Instance = this;
        Inputs = new();

        Inputs.Mouse.Enable();
        Inputs.Mouse.LeftClick.performed += (_) => CheckForClick();
    }

    void CheckForClick()
    {
        var mousePos = Helpers.GetMouseWorldPosition();
        var cellUnderMouse = WorldManager.World.GetCellAt(mousePos.ToVector3Int());

        Debug.Log(cellUnderMouse.Coords);
    }

    //private void controllaClick()
    //{
    //    Vector2 pos=Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //    RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero);
    //    if (hit.collider != null)
    //    {
    //        MineralNode mine = hit.collider.GetComponent<MineralNode>();
    //        if (mine != null)
    //        {
    //            botClicked = true;
    //            mine.colpisci();
    //        }
    //        else botClicked = false;
    //    }
    //    else botClicked = false;
    //}
}
