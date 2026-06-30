using UnityEngine;

public static class TransformExtensions
{
    /// <summary>
    /// return 1 is right, -1 is left, 0 is front.
    /// </summary>
    public static int GetDirectionToTarget(this Transform transform, Vector3 targetPosition)
    {
        Vector3 directionToTarget = (targetPosition - transform.position).normalized;
        Vector3 crossProduct = Vector3.Cross(transform.forward, directionToTarget);

        float dotResult = Vector3.Dot(crossProduct, transform.up);
        if (dotResult > Mathf.Epsilon)
            return 1;
        if (dotResult < -Mathf.Epsilon)
            return -1;

        return 0;
    }
}
