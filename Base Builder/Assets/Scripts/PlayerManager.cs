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

        Inputs.Player.Movement.Enable();
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
        vec *= speed * Time.deltaTime;
        transform.position += (Vector3)vec;
    }
}
