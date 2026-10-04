using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] Camera camera;
    [Space]
    [SerializeField][Tooltip("0.04f for normal speed.")] float moveSpeed = 0.04f;
    [SerializeField][Tooltip("2f for normal speed.")] float zoomSpeed = 2f;
    [SerializeField] float minZoom = 2f;
    [SerializeField] float maxZoom = 10f;

    Bounds cameraBounds;
    
    private void Awake()
    {
        PlayerManager.Inputs.Camera.Enable();
        PlayerManager.Inputs.Camera.Delta.performed += Move;
        PlayerManager.Inputs.Camera.Scroll.performed += Zoom;
    }
    private void FixedUpdate()
    {
        cameraBounds.center = transform.position + new Vector3(0, 0, 10);
        cameraBounds.size = new Vector3(camera.orthographicSize * 2 * camera.aspect, camera.orthographicSize * 2, 0);
    }
    private void Move(InputAction.CallbackContext context)
    {
        Vector3 lastPos = transform.position;

        if (!PlayerManager.Inputs.Camera.LeftClick.IsInProgress())
            return;
        
        Vector3 delta = context.ReadValue<Vector2>();
        transform.position -= delta * moveSpeed;

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
