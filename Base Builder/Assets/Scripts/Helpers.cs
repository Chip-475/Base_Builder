using UnityEngine;

public static class Helpers
{
    #region Vector Helpers
    public static Vector2 ToVector2(this Vector2Int vec)
    {
        return new Vector2(vec.x, vec.y);
    }
    public static Vector2Int ToVector2Int(this Vector2 vec)
    {
        int X = Mathf.RoundToInt(vec.x);
        int Y = Mathf.RoundToInt(vec.y);
        return new Vector2Int(X, Y);
    }
    public static Vector2Int ToVector2Int(this Vector3Int vec)
    {
        return new Vector2Int(vec.x, vec.y);
    }
    #endregion
}
