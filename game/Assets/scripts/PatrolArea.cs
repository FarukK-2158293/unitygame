using UnityEngine;
using System.Collections.Generic;

public class PatrolArea : MonoBehaviour
{
    [Header("Area Point Transforms")]
    public List<Transform> patrolPoints = new List<Transform>();

    [Header("Visualization")]
    public bool showAreaInEditor = true;
    public Color areaColor = Color.green;

    void Start()
    {
        if (patrolPoints.Count < 3)
        {
            Debug.LogWarning("PatrolArea needs at least 3 points to define an area!");
        }
    }

    public bool IsPointInside(Vector2 point)
    {
        if (patrolPoints.Count < 3 || !AllPointsAssigned()) return true;

        int intersections = 0;
        for (int i = 0; i < patrolPoints.Count; i++)
        {
            Vector2 p1 = patrolPoints[i].position;
            Vector2 p2 = patrolPoints[(i + 1) % patrolPoints.Count].position;

            if (((p1.y > point.y) != (p2.y > point.y)) &&
                (point.x < (p2.x - p1.x) * (point.y - p1.y) / (p2.y - p1.y) + p1.x))
            {
                intersections++;
            }
        }
        return (intersections % 2) == 1;
    }

    public Vector2 GetRandomPointInside()
    {
        if (patrolPoints.Count < 3 || !AllPointsAssigned())
        {
            return transform.position + (Vector3)(Random.insideUnitCircle * 5f);
        }

        Vector2 min = patrolPoints[0].position;
        Vector2 max = patrolPoints[0].position;

        foreach (Transform point in patrolPoints)
        {
            Vector2 pos = point.position;
            min = Vector2.Min(min, pos);
            max = Vector2.Max(max, pos);
        }

        Vector2 randomPoint;
        int attempts = 0;
        do
        {
            randomPoint = new Vector2(
                Random.Range(min.x, max.x),
                Random.Range(min.y, max.y)
            );
            attempts++;
        } while (!IsPointInside(randomPoint) && attempts < 50);

        return randomPoint;
    }

    public Vector2 GetNearestPointInside(Vector2 outsidePoint)
    {
        if (patrolPoints.Count < 3 || !AllPointsAssigned())
        {
            return transform.position;
        }

        Vector2 nearestPoint = patrolPoints[0].position;
        float nearestDistance = Vector2.Distance(outsidePoint, nearestPoint);

        for (int i = 0; i < patrolPoints.Count; i++)
        {
            Vector2 p1 = patrolPoints[i].position;
            Vector2 p2 = patrolPoints[(i + 1) % patrolPoints.Count].position;

            Vector2 pointOnEdge = GetNearestPointOnLine(p1, p2, outsidePoint);
            float distance = Vector2.Distance(outsidePoint, pointOnEdge);

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestPoint = pointOnEdge;
            }
        }

        return nearestPoint;
    }

    Vector2 GetNearestPointOnLine(Vector2 lineStart, Vector2 lineEnd, Vector2 point)
    {
        Vector2 line = lineEnd - lineStart;
        float length = line.magnitude;
        line.Normalize();

        Vector2 v = point - lineStart;
        float d = Vector2.Dot(v, line);
        d = Mathf.Clamp(d, 0f, length);

        return lineStart + line * d;
    }

    bool AllPointsAssigned()
    {
        foreach (Transform point in patrolPoints)
        {
            if (point == null) return false;
        }
        return patrolPoints.Count > 0;
    }

    void OnDrawGizmos()
    {
        if (!showAreaInEditor || patrolPoints.Count < 3 || !AllPointsAssigned()) return;

        Gizmos.color = areaColor;
        for (int i = 0; i < patrolPoints.Count; i++)
        {
            Vector3 current = patrolPoints[i].position;
            Vector3 next = patrolPoints[(i + 1) % patrolPoints.Count].position;
            Gizmos.DrawLine(current, next);
            Gizmos.DrawWireSphere(current, 0.3f);

            // Draw point numbers
            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(current + Vector3.up * 0.5f, Vector3.one * 0.2f);
            Gizmos.color = areaColor;
        }

        // Draw area fill
        Gizmos.color = new Color(areaColor.r, areaColor.g, areaColor.b, 0.1f);
    }

    void OnValidate()
    {
        if (patrolPoints.Count > 0 && AllPointsAssigned())
        {
            // Validation passed
        }
    }
}
