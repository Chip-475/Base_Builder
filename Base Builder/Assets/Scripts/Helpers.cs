using UnityEngine;
using UnityEngine.InputSystem;
using System;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;

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
    #endregion
    #region Cell Helpers
    public static bool Overlaps(this Cell[] a, Cell[] b)
    {
        foreach (Cell other in a)
            foreach (Cell other2 in b)
                if (other == other2) return true;

        return false;
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
