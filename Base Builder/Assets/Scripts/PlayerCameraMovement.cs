using UnityEngine;

public class PlayerCameraMovement : MonoBehaviour
{
    public static PlayerManager Player { get; set; }
    public Camera Camera { get; private set; }

    private void Awake() => Camera = Camera.main;
    private void LateUpdate()
    {
        Vector3 playerPos = Player.transform.position;
        playerPos.z = -10;
        Camera.transform.position = playerPos;
    }
}
