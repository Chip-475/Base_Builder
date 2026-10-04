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

        Inputs.CameraDrag.LeftClick.performed += (input) => CheckForClick();
    }

    void CheckForClick()
    {
        var mousePos = Helpers.GetMouseWorldPosition();
        var cellUnderMouse = WorldManager.World.GetCellAt(mousePos.ToVector3Int());

        Debug.Log(cellUnderMouse.Coords);
    }
}
