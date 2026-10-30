using System;
using UnityEngine;
using UnityEngine.InputSystem;
[DefaultExecutionOrder(-1)]
public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }
    public static PlayerInputs Inputs { get; private set; }

    [Header("Settings")]
    [SerializeField][Tooltip("7f standard.")] float speed = 7f;

    [Header("Setup")]
    [SerializeField] SpriteRenderer sr;
    public Transform botCarryPoint;
    public Transform botDropPoint;

    [Header("Runtime")]
    public bool IsCarryingBot { get; set; }

    void Awake()
    {
        Instance = this;
        Inputs = new();
        PlayerCameraMovement.Player = this;
        PlayerMode();
        Inputs.Player.Interact.performed += Interact;
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
        if (!nextCell.canWalkOn || nextCell == null)
            return;

        vec *= speed * Time.deltaTime;
        transform.position += (Vector3)vec;
        if (vec.x == 0)
            return;
        transform.localScale = new(MathF.Sign(vec.x), transform.localScale.y);
    }
    public void Interact(InputAction.CallbackContext context)
    {
        Debug.Log("pressed interact");
        if (InteractionManager.Instance.currentInteractable == null) Debug.Log("no interactable found");
        else Debug.Log("Interactable found:" + InteractionManager.Instance.currentInteractable);
            InteractionManager.Instance.currentInteractable.Interact();
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
