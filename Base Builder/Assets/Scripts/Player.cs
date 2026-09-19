using UnityEngine;

public class Player: MonoBehaviour
{
    public static Player Instance {  get; private set; }
    public static PlayerInputs PlayerInputs { get; private set; }

    public bool botClicked;
    public bool buildMode;

    void Awake()
    {
        Instance = this;
        PlayerInputs = new();

        PlayerInputs.Player.Enable();
        PlayerInputs.Player.LeftClick.performed += (_) => CheckForClick();
    }

    void CheckForClick()
    {
        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
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
