using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance {  get; private set; }
    public static PlayerInputs Inputs { get; private set; }

    void Awake()
    {
        Instance = this;
        Inputs = new();

        Inputs.CameraDrag.Enable();
        Inputs.CameraDrag.LeftClick.performed += (input) => { CheckForClick(); Direction(input); };
        Inputs.CameraDrag.CameraMovement.performed += (input) => Direction(input);
        Inputs.CameraDrag.CameraMovement.canceled += (_) => DeleteDirection();
    }

    void CheckForClick()
    {
        var mousePos = Helpers.GetMouseWorldPosition();
        var cellUnderMouse = WorldManager.World.GetCellAt(mousePos.ToVector3Int());

        Debug.Log(cellUnderMouse.Coords);
    }
    public void DeleteDirection()
    {
        CameraMovement.direction = Vector2.zero;
    }
    public static void Direction(InputAction.CallbackContext context)
    {
        if(Inputs.CameraDrag.LeftClick.IsPressed())
        {
            Debug.Log("dio cazzo");
           CameraMovement.direction=Inputs.CameraDrag.Delta.ReadValue<Vector2>()*(-0.1f);
            return;
        }
        CameraMovement.direction = context.ReadValue<Vector2>();
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
