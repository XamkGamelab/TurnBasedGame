using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Helper class for basic math methods
/// </summary>
public class MathHelper
{
    public static Vector3 CompassDirectionFromVec3(Vector3 vector3Direction)
    {
        List<Vector3> compassDirections = new List<Vector3>() { Vector3.left, Vector3.right, Vector3.forward, Vector3.back, Vector3.up, Vector3.down };

        float maxDot = -Mathf.Infinity;
        Vector3 closestDir = Vector3.zero;

        // If you take the dot products between the compass direction vectors and the unknown vector,
        // the highest value will indicate, which vector is closest to unknown direction.
        compassDirections.ForEach(direction =>
        {
            var t = Vector3.Dot(vector3Direction, direction);
            if (t > maxDot)
            {
                closestDir = direction;
                maxDot = t;
            }
        });

        return closestDir;
    }
}