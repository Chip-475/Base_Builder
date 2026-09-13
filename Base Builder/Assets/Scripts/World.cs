using UnityEngine;

public class World
{
    public World(Vector2Int size)
    {
        cells = new Cell[size.x, size.y];
    }

    Cell[,] cells;
}
