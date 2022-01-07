using UnityEngine;

namespace ExtensionMethods;

public static class VectorExtensions
{
    public static Vector3 RotateAround(this Vector3 vector, Vector3 axis, float angle)
    {
        return Quaternion.AngleAxis(angle, axis) * vector;
    }
}