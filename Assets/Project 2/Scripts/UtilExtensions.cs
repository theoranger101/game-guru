using UnityEngine;

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
}