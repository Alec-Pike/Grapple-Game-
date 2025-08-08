using UnityEngine;
using System.Linq;

/*
* Utility for ConeCastExtension written by Grok.
*/

public static class ConeCastUtility
{
    public static bool FindClosestToConeCenter(RaycastHit[] coneCastHits, Vector3 origin, Vector3 direction, LayerMask targetLayer, out RaycastHit closestHit)
    {
        closestHit = new RaycastHit();
        direction = direction.normalized;
        float smallestAngle = float.MaxValue;

        // Filter hits by layer and find the one with the smallest angle to the center
        foreach (var hit in coneCastHits)
        {
            // Check if the hit is on the specified layer
            if (((1 << hit.collider.gameObject.layer) & targetLayer) != 0)
            {
                // Calculate the angle between the cone's direction and the hit point
                Vector3 directionToHit = (hit.point - origin).normalized;
                float angleToHit = Vector3.Angle(direction, directionToHit);

                // Update closest hit if this angle is smaller
                if (angleToHit < smallestAngle)
                {
                    smallestAngle = angleToHit;
                    closestHit = hit;
                }
            }
        }

        // Return true if a valid hit was found
        return smallestAngle != float.MaxValue;
    }
}