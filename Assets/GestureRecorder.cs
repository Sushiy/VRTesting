using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class GestureRecorder : MonoBehaviour {

    [SerializeField]
    private float m_fSphereRadius = 0.3f;
    [SerializeField]
    [Range(0,360)]
    private int m_iMaxAngle = 20;
    [SerializeField]
    [Range(0f, 2f)]
    private float m_fMinimalDistance = 0.2f;
    [SerializeField]
    [Range(1, 10)]
    private int m_iRequiredNumber = 5;

    private List<Vector3> m_arrPoints = new List<Vector3>(); // just for the line renderer
    private List<Vector2> m_arrPoints2D = new List<Vector2>();
    private List<Vector2> m_arrCharPoints = new List<Vector2>();
    private List<Vector2> m_arrAddedPoints = new List<Vector2>();

    private LineRenderer line;
    private bool newLine = true;

    void Awake()
    {
        line = GetComponent<LineRenderer>();
    }
    
    /// <summary>
    /// Drawing and recording gestures!
    /// </summary>
	void Update () {
        if (Input.GetMouseButton(0))
        {
            if (newLine)
            {
                m_arrPoints.Clear();
                m_arrPoints2D.Clear();
                m_arrCharPoints.Clear();
                newLine = false;
            }

            float x = Input.mousePosition.x;
            float y = Input.mousePosition.y;
            Vector3 p = Camera.main.ScreenToWorldPoint(new Vector3(x, y, 10));
            m_arrPoints.Add(p);
            m_arrPoints2D.Add(p);

            line.SetVertexCount(m_arrPoints.Count);
            line.SetPositions(m_arrPoints.ToArray());
        }

        if (Input.GetMouseButtonUp(0))
        {
            Vector2[] charp;
            Vector2[] p = m_arrPoints2D.ToArray();
            IdentifyCharPoints(ref p, out charp, m_iRequiredNumber);
            m_arrCharPoints.AddRange(charp);
            newLine = true;
        }
	}

    /// <summary>
    /// Takes in an array of 2D points and returns
    /// the characteristic points of the point set.
    /// </summary>
    /// <param name="p">The point list</param>
    /// <param name="charp">The returned characteristic points</param>
    void IdentifyCharPoints(ref Vector2[] p, out Vector2[] charp, int requiredNr)
    {
        List<Vector2> result = new List<Vector2>(); 
        Vector2 lastCharPoint = p[0];
        result.Add(lastCharPoint);
        
        /* Step 1: Find the characteristic points */

        for (int i=1; i<m_arrPoints.Count-1; ++i)
        {
            // Pass the minimal distance test?
            float distance = Vector2.Distance(p[i], lastCharPoint);
            if (distance < m_fMinimalDistance)
                continue;

            // Pass the angle test?
            Vector2 segment1 = p[i] - lastCharPoint;
            Vector2 segment2 = p[i + 1] - p[i];
            float angle = Vector2.Angle(segment1.normalized, segment2.normalized);
            if (angle < m_iMaxAngle)
                continue;

            lastCharPoint = p[i];
            result.Add(lastCharPoint);
        }

        // fix the last point
        Vector2 lastPoint = p[p.Length - 1];

        // is it already the last point? you're good! Else...
        if (lastCharPoint != lastPoint)
        {
            // is the last added point too close? remove the lastCharPoint and replace by last point
            float dist = Vector2.Distance(lastCharPoint, lastPoint);
            if (dist < m_fMinimalDistance)
                result[result.Count - 1] = lastPoint;
        }

        m_arrAddedPoints.Clear(); // TAKE THIS OUT

        /* Step 2: Increase points? */
        if (result.Count < requiredNr)
        {
            int diff = requiredNr - result.Count;
            print("Adding " + diff + " points");
            AddPointsToLongestSegments(ref result, diff);
        }
        /* Step 3: Decrease points? */
        else if (result.Count > requiredNr)
        {
            // erase some
        }


        // Last Step: Assign the result
        charp = result.ToArray();
    }

    /// <summary>
    /// Itieratively adds points to the longest segmets
    /// (probably can be optimized!)
    /// </summary>
    /// <param name="p">The point list where points will be added</param>
    /// <param name="number">The number of points that shall be added</param>
    void AddPointsToLongestSegments(ref List<Vector2> p, int number)
    {
        // Check for traps
        if (p.Count <= 1)
        {
            print("Not a single segment to add points to! Please record more points!");
            return;
        }

        // Add the points
        for (int n=0; n<number; ++n)
        {
            // Init for a point
            float maxDistance = float.MinValue;
            int segmentStart = -1;

            // find the longest distance
            for (int i=0; i < p.Count - 1; ++i)
            {
                float distance = Vector2.Distance(p[i], p[i + 1]);
                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    segmentStart = i;
                }
            }

            // add a single point
            Vector2 newPoint = Vector2.Lerp(p[segmentStart], p[segmentStart + 1], 0.5f);

            m_arrAddedPoints.Add(newPoint);

            p.Insert(segmentStart + 1, newPoint);
        }

        print("Total of " + p.Count + " points after adding");
    }

    // Drawing Spheres where characteristic Points are
    void OnDrawGizmos()
    {
        if (m_arrCharPoints.Count == 0)
            return;

        float r = 0f;
        float g = 0f;
        float b = 0f;

        float colorStep = 1f / ((float)m_arrCharPoints.Count - 2);

        // Draw the characteristic points
        foreach (Vector3 p in m_arrCharPoints)
        {
            Gizmos.color = new Color(r, g, b);
            Gizmos.DrawSphere(new Vector3(p.x, p.y, 0), m_fSphereRadius);
            r += colorStep;
            g += colorStep;
            b += colorStep;
        }

        // Draw the added points
        foreach (Vector3 p in m_arrAddedPoints)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(new Vector3(p.x, p.y, 0), m_fSphereRadius + 0.2f);
        }
    }
}
