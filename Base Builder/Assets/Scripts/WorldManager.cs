using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldManager : MonoBehaviour
{
    public static WorldManager instance;

    [SerializeField] Grid grid;
    World world;

    public Sprite floorSprite;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }
    void Start()
    {
        grid.gameObject.SetActive(false);
        world = new(grid);
    }
}
