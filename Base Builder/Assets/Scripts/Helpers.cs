using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public static class Helpers
{
    #region Float Helpers
    public static int ToMilliseconds(this float value)
    {
        return Mathf.RoundToInt(value * 1000);
    }
    #endregion
    #region Vector Helpers
    public static Vector3 ToVector3(this Vector3Int vec)
    {
        return new Vector3(vec.x, vec.y, vec.z);
    }
    public static Vector3Int ToVector3Int(this Vector3 vec)
    {
        int X = Mathf.RoundToInt(vec.x);
        int Y = Mathf.RoundToInt(vec.y);
        int Z = Mathf.RoundToInt(vec.z);
        return new Vector3Int(X, Y, Z);
    }
    public static Vector3Int[] GetNeighbours(this Vector3Int vec)
    {
        return new Vector3Int[]
        {
            new(vec.x, vec.y + 1, 0),
            new(vec.x + 1, vec.y, 0),
            new(vec.x, vec.y - 1, 0),
            new(vec.x - 1, vec.y, 0)
        };
    }
    #endregion
    #region Cell Helpers
    public static bool Overlaps(this Cell[] a, Cell[] b)
    {
        foreach (Cell other in a)
            foreach (Cell other2 in b)
                if (other == other2) return true;

        return false;
    }
    public static Cell[] GetCellsInBounds(Bounds bounds)
    {
        int minX = Mathf.CeilToInt(bounds.min.x);
        int maxX = Mathf.FloorToInt(bounds.max.x);
        int minY = Mathf.CeilToInt(bounds.min.y);
        int maxY = Mathf.FloorToInt(bounds.max.y);

        List<Cell> cells = new();
        for (int x = minX; x <= maxX; x++)
            for (int y = minY; y <= maxY; y++)
            {
                Cell cell = WorldManager.World.GetCellAt(new Vector3Int(x, y, 0));
                cells.Add(cell);
            }

        return cells.ToArray();
    }
    #endregion
    #region Direction Helpers
    public static Directions Opposite(this Directions direction)
    {
        return direction switch
        {
            Directions.Up => Directions.Down,
            Directions.Right => Directions.Left,
            Directions.Down => Directions.Up,
            Directions.Left => Directions.Right,
            _ => throw new Exception("Direction is invalid.")
        };
    }
    public static Directions Rotate(this Directions direction, int amount)
    {
        var newDirection = direction;
        newDirection += amount;
        if (newDirection == 0)
            newDirection += Math.Sign(amount);

        return newDirection;
    }
    public static Vector3Int ToVector(this Directions direction)
    {
        return direction switch
        {
            Directions.Up => Vector3Int.up,
            Directions.Right => Vector3Int.right,
            Directions.Down => Vector3Int.down,
            Directions.Left => Vector3Int.left,
            _ => throw new Exception("Direction is invalid.")
        };
    }
    #endregion

    #region Building Helpers
    public static bool CanBuildOn(this BuildingView prefab, Vector3Int gridPos)
    {
        if (prefab == null || prefab.Data == null)
            return false;

        if (WorldManager.Instance == null || WorldManager.World == null)
            return false;

        Bounds bounds = prefab.Data.bounds;
        bounds.center = gridPos;

        foreach (Cell cell in GetCellsInBounds(bounds))
            if (cell == null || !cell.canBuildOn) return false;

        return true;
    }
    #endregion

    #region Miscellaneous
    public static Vector3 GetMousePosition()
    {
        return Mouse.current.position.ReadValue();
    }
    public static Vector3 GetMouseWorldPosition()
    {
        var pos = Camera.main.ScreenToWorldPoint(GetMousePosition());
        pos.z = 0;
        return pos;
    }
    public static bool IsMouseOverUI()
    {
        PointerEventData pointer =
            new(EventSystem.current)
            {
                position = Mouse.current.position.ReadValue()
            };

        List<RaycastResult> results = new();
        EventSystem.current.RaycastAll(pointer, results);

        return results.Exists(result =>
            result.module is GraphicRaycaster);
    }
    #endregion
}
public enum Directions
{
    None,
    Up,
    Right,
    Down,
    Left,
    UpLeft,
    UpRight,
    DownLeft,
    DownRight
}
