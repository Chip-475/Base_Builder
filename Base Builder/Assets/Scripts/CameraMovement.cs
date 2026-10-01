using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
     public float moveSpeed = 5f;
     public float zoomSpeed = 2f;
     public float minZoom = 2f;
     public float maxZoom = 10f;
    public static Vector2 direction;
    private Camera cam;
    void Start()
    {
        cam = GetComponent<Camera>();
    }
    void Update()
    {
        HandleMovement();
        HandleZoom();
        //Debug.Log($"DirectionX: {direction.x}, DirectionY: {direction.y}");
    }
    private void HandleMovement()
    {
        float moveX = direction.x;
        float moveY = direction.y;
        Debug.Log($"MoveX: {moveX}, MoveY: {moveY}");
        Vector3 move = new Vector3(moveX, moveY, 0) * moveSpeed * Time.deltaTime;
        transform.position += move;
    }
    private void HandleZoom()
    {
        float scroll = Mouse.current.scroll.y.ReadValue();
        if (scroll != 0)
        {
            cam.orthographicSize -= scroll * zoomSpeed;
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
        }
    }
}
