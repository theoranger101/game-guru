using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public static class UtilExtensions
{
    public static bool IsWithin(this int value, int minimum, int maximum)
    {
        return value >= minimum && value <= maximum;
    }

    public static bool IsWithin(this float value, float minimum, float maximum)
    {
        return value >= minimum && value <= maximum;
    }

    public static bool IsWithin(this float value, float minimum, float maximum, float tolerance)
    {
        return value >= (minimum - tolerance) && value <= (maximum + tolerance);
    }
    
    public static Vector3 WithX(this Vector3 v, float x)
    {
        v.x = x;
        return v;
    }

    public static Vector3 WithY(this Vector3 v, float y)
    {
        v.y = y;
        return v;
    }

    public static Vector3 WithZ(this Vector3 v, float z)
    {
        v.z = z;
        return v;
    }

    public static Vector3 WithXY(this Vector3 v, float x, float y)
    {
        v.x = x;
        v.y = y;
        return v;
    }

    public static Vector3 WithXZ(this Vector3 v, float x, float z)
    {
        v.x = x;
        v.z = z;
        return v;
    }

    public static Vector3 WithYZ(this Vector3 v, float y, float z)
    {
        v.y = y;
        v.z = z;
        return v;
    }
    
    public static Material GetRandomMaterial(this List<Material> materials)
    {
        var rand = new Random();
        var randIndex = rand.Next(materials.Count);

        return materials[randIndex];
    }
}