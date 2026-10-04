using System.IO;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Rendering;
public class CameraMovement : MonoBehaviour
{
     public float moveSpeed = 5f;
     public float zoomSpeed = 2f;
     public float minZoom = 2f;
     public float maxZoom = 10f;
    public static Vector2 direction;
    private Camera cam;
    private bool isDragging;
    private Vector3 savedMousePos;
    private Vector3Int bottomLeft { get { return cam.ViewportToWorldPoint(new Vector3(0,0,0)).ToVector3Int() + new Vector3Int(0, 0, 10) ; } }
    private Vector3Int topLeft { get { return cam.ViewportToWorldPoint(new Vector3(0, 1, 0)).ToVector3Int() + new Vector3Int(0, 0, 10) ; } }
    private Vector3Int bottomRight { get { return cam.ViewportToWorldPoint(new Vector3(1, 0, 0)).ToVector3Int() + new Vector3Int(0, 0, 10); } }
    private Vector3Int topRight { get { return cam.ViewportToWorldPoint(new Vector3(1, 1, 0)).ToVector3Int() + new Vector3Int(0, 0, 10) ; } }

    private Bounds cameraBounds = new Bounds();
    
    void Update()
    {
        HandleMovement();
        HandleZoom();
    }
    private void Awake()
    {
        cam = GetComponent<Camera>();
        PlayerManager.Inputs.CameraDrag.Delta.performed += CameraMove;
    }
    private void FixedUpdate()
    {
        cameraBounds.center = transform.position + new Vector3(0, 0, 10);
        cameraBounds.size = new Vector3(cam.orthographicSize * 2 * cam.aspect, cam.orthographicSize * 2, 0);
    }
    private void OnDrawGizmos()
    {
        if (cam == null) return;
        cameraBounds.center = transform.position+new Vector3(0,0,10);
        cameraBounds.size = new Vector3(cam.orthographicSize * 2 * cam.aspect, cam.orthographicSize * 2, 0);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(cameraBounds.center, cameraBounds.size);
    }
    private void HandleMovement()
    {
        float moveX = direction.x;
        float moveY = direction.y;
        if (cameraBounds.Intersects(WorldManager.World.WorldBounds)) { Debug.Log("intersect"); return; }
        Vector3 move = new Vector3(moveX, moveY, 0) * moveSpeed * Time.deltaTime;
        Directions dir;
        if(move.x>0&& move.y>0) dir = Directions.UpRight;
        else if(move.x>0 && move.y<0) dir = Directions.DownRight;
        else if(move.x<0 && move.y>0) dir = Directions.UpLeft;
        else if(move.x<0 && move.y<0) dir = Directions.DownLeft;
        else if(move.x>0) dir = Directions.Right;
        else if(move.x<0) dir = Directions.Left;
        else if(move.y>0) dir = Directions.Up;
        else if(move.y<0) dir = Directions.Down;
        else return;
        switch (dir)
        {
            case Directions.Up:
                if (WorldManager.World.HasCellAt(topRight + move.ToVector3Int()) || WorldManager.World.HasCellAt(topLeft + move.ToVector3Int())) { Debug.Log("moving"); transform.position += move; }
                break;
            case Directions.Down:
                if (WorldManager.World.HasCellAt(bottomRight + move.ToVector3Int()) || WorldManager.World.HasCellAt(bottomLeft + move.ToVector3Int())) { Debug.Log("moving"); transform.position += move; }
                break;
            case Directions.Left:
                if (WorldManager.World.HasCellAt(topLeft + move.ToVector3Int()) || WorldManager.World.HasCellAt(bottomLeft + move.ToVector3Int())) { Debug.Log("moving"); transform.position += move; }
                break;
            case Directions.Right:
                if (WorldManager.World.HasCellAt(topRight + move.ToVector3Int()) || WorldManager.World.HasCellAt(bottomRight + move.ToVector3Int())) { Debug.Log("moving"); transform.position += move; }
                break;
            case Directions.UpRight:
                if (WorldManager.World.HasCellAt(topRight + move.ToVector3Int()) || WorldManager.World.HasCellAt(bottomRight + move.ToVector3Int()) || WorldManager.World.HasCellAt(topLeft + move.ToVector3Int())) { Debug.Log("moving"); transform.position += move; }
                break;
            case Directions.UpLeft:
                if (WorldManager.World.HasCellAt(topLeft + move.ToVector3Int()) || WorldManager.World.HasCellAt(bottomLeft + move.ToVector3Int()) || WorldManager.World.HasCellAt(topRight + move.ToVector3Int())) { Debug.Log("moving"); transform.position += move; }
                break;
            case Directions.DownRight:
                if (WorldManager.World.HasCellAt(bottomRight + move.ToVector3Int()) || WorldManager.World.HasCellAt(topRight + move.ToVector3Int()) || WorldManager.World.HasCellAt(bottomLeft + move.ToVector3Int())) { Debug.Log("moving"); transform.position += move; }
                break;
            case Directions.DownLeft:
                if (WorldManager.World.HasCellAt(bottomLeft + move.ToVector3Int()) || WorldManager.World.HasCellAt(topLeft + move.ToVector3Int()) || WorldManager.World.HasCellAt(bottomRight + move.ToVector3Int())) { Debug.Log("moving"); transform.position += move; }
                break;

        }
    }
    private void CameraMove(InputAction.CallbackContext context)
    {
        if (!PlayerManager.Inputs.CameraDrag.LeftClick.IsInProgress()) return;
        if (!cameraBounds.Intersects(WorldManager.World.WorldBounds)) { Debug.Log("intersect"); return; }
        else {Debug.Log("not intersect");}
        Vector2 d = context.ReadValue<Vector2>();
        Vector3 delta = new Vector3(d.x, d.y, 0);
        transform.position -= delta*0.05f;
    }
    /*private void HandleMovementWithMouse()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            isDragging = true;
            savedMousePos = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        }
        if (Mouse.current.leftButton.isPressed && isDragging)
        {
            Vector3 currentMousePosition=cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            Vector3 movement = savedMousePos - currentMousePosition;
            transform.position += movement;
            savedMousePos = currentMousePosition;
        }
        if(Mouse.current.leftButton.wasReleasedThisFrame)
        {
            isDragging = false;
        }
    }*/
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
