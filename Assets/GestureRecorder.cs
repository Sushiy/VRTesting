using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class GestureRecorder : MonoBehaviour {

    [SerializeField]
    private float sphereRadius = 0.3f;
    [SerializeField]
    [Range(0,360)]
    private int maxAngle = 20;
    [SerializeField]
    [Range(0f, 2f)]
    private float minimalDistance = 0.2f;

    private List<Vector3> points = new List<Vector3>();
    private List<Vector2> points2 = new List<Vector2>();
    private List<Vector2> charPoints = new List<Vector2>();


    private LineRenderer line;
    private bool newLine = true;

    void Awake()
    {
        line = GetComponent<LineRenderer>();
    }
    
    // Drawing Gestures
	void Update () {
        if (Input.GetMouseButton(0))
        {
            if (newLine)
            {
                points.Clear();
                points2.Clear();
                charPoints.Clear();
                newLine = false;
            }

            float x = Input.mousePosition.x;
            float y = Input.mousePosition.y;
            Vector3 p = Camera.main.ScreenToWorldPoint(new Vector3(x, y, 10));
            points.Add(p);
            points2.Add(p);

            line.SetVertexCount(points.Count);
            line.SetPositions(points.ToArray());
        }

        if (Input.GetMouseButtonUp(0))
        {
            IdentifyCharPoints();
            newLine = true;
        }
	}

    void IdentifyCharPoints()
    {
        Vector2 lastCharPoint = points2[0];
        charPoints.Add(lastCharPoint);

        for (int i=1; i<points.Count-1; ++i)
        {
            // Pass the minimal distance test?
            float distance = Vector2.Distance(points2[i], lastCharPoint);
            if (distance < minimalDistance)
                continue;

            // Pass the angle test?
            Vector2 segment1 = points2[i] - lastCharPoint;
            Vector2 segment2 = points2[i + 1] - points2[i];
            float angle = Vector2.Angle(segment1.normalized, segment2.normalized);
            if (angle < maxAngle)
                continue;

            lastCharPoint = points2[i];
            charPoints.Add(lastCharPoint);
        }

        // fix the last point
        Vector2 lastPoint = points2[points2.Count - 1];

        // is it already the last point? you're good!
        if (lastCharPoint == lastPoint)
            return;

        // is the last added point too close? remove the lastCharPoint and replace by last point
        float dist = Vector2.Distance(lastCharPoint, lastPoint);
        if (dist < minimalDistance)
            charPoints[charPoints.Count - 1] = lastPoint;
    }

    // Drawing Spheres where characteristic Points are
    void OnDrawGizmos()
    {
        if (charPoints.Count == 0)
            return;

        Gizmos.color = Color.red;
        foreach (Vector3 p in charPoints)
        {
            Gizmos.DrawSphere(new Vector3(p.x, p.y, 0), sphereRadius);
        }
    }
}
