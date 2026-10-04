using System.IO;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Rendering;
using UnityEngine.Windows;

public class CameraMovement : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] Camera camera;
    [Space]
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float zoomSpeed = 2f;
    [SerializeField] float minZoom = 2f;
    [SerializeField] float maxZoom = 10f;

    Bounds cameraBounds = new();
    Vector3 lastPos;
    
    private void Awake()
    {
        PlayerManager.Inputs.CameraDrag.Enable();
        PlayerManager.Inputs.CameraDrag.Delta.performed += Move;
    }
    private void FixedUpdate()
    {
        cameraBounds.center = transform.position + new Vector3(0, 0, 10);
        cameraBounds.size = new Vector3(camera.orthographicSize * 2 * camera.aspect, camera.orthographicSize * 2, 0);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(cameraBounds.center, cameraBounds.size);
    }
    private void Move(InputAction.CallbackContext context)
    {
        Vector3 lastPos = transform.position;

        if (!PlayerManager.Inputs.CameraDrag.LeftClick.IsInProgress())
            return;
        
        Vector3 delta = context.ReadValue<Vector2>();
        transform.position -= delta * 0.05f;

        if (IsOutOfBounds())
            transform.position = lastPos;
    }
    private void Zoom(InputAction.CallbackContext context)
    {
        Vector2 scrollValue = context.ReadValue<Vector2>();

        camera.orthographicSize -= scrollValue.y * zoomSpeed;
        camera.orthographicSize = Mathf.Clamp(camera.orthographicSize, minZoom, maxZoom);
    }

    bool IsOutOfBounds()
    {
        Vector3 cameraPos = transform.position;
        cameraPos.z = 0;

        if (!WorldManager.World.WorldBounds.Contains(cameraPos))
            return true;
        if (!cameraBounds.Intersects(WorldManager.World.WorldBounds))
            return true;

        return false;
    }
}
