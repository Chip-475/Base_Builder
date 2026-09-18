using UnityEngine;

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
}
