using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance {  get; private set; }
    public static PlayerInputs Inputs { get; private set; }

    [Header("Settings")]
    [SerializeField][Tooltip("7f standard.")] float speed = 7f;

    [Header("Components")]
    [SerializeField] SpriteRenderer sr;

    void Awake()
    {
        Instance = this;
        PlayerCameraMovement.Player = this;
        Inputs = new();

        PlayerMode();
    }
    private void Update()
    {
        if (Inputs.Player.Movement.IsPressed())
        {
            Vector2 vec = Inputs.Player.Movement.ReadValue<Vector2>();
            Move(vec);
        }
    }

    public void Move(Vector2 vec)
    {
        Cell nextCell =
            WorldManager.World.GetCellAt((transform.position + (Vector3)(speed * Time.deltaTime * vec)).ToVector3Int());
        if (!nextCell.canWalkOn)
            return;

        vec *= speed * Time.deltaTime;
        transform.position += (Vector3)vec;
    }

    public Cell GetPlayerCell()
    {
        return WorldManager.World.GetCellAt(transform.position.ToVector3Int());
    }

    // Input Managing
    public static void DisableAll()
    {
        Inputs.Player.Disable();
        Inputs.Camera.Disable();
        Inputs.BuildMode.Disable();
    }
    public static void PlayerMode()
    {
        DisableAll();
        Inputs.Player.Enable();
    }
    public static void BuildMode()
    {
        DisableAll();
        Inputs.Camera.Enable();
        Inputs.BuildMode.Enable();
    }
    public static void FreeLookMode()
    {
        DisableAll();
        Inputs.Camera.Enable();
    }
}
