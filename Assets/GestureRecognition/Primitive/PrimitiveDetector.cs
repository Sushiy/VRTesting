using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using gestureUtil;

namespace primitive
{
    /// <summary>
    /// Records the movements of the desired object (mouse, wand)
    /// and detects if simple primitives were drawn (big enough)
    /// 
    /// TODO make a queue of points with the last few seconds/hundred
    /// points
    /// TODO detect edges and count them
    /// TODO check if edges "overlapped" and see if the thing is big enough
    /// </summary>
    public class PrimitiveDetector : MonoBehaviour
    {
        [SerializeField]
        private float m_fDequeueInterval = 0.5f;
        [SerializeField]
        private int m_iPointCount = 256;
        [SerializeField]
        private float m_fMinimumDistance = 0.25f;
        [SerializeField]
        private int m_iMinimumIndexDiff = 40;
        [SerializeField]
        [Range(0f, 1f)]
        private float m_fRadiusTolerance = 0.25f;
        [SerializeField]
        [Range(0f, 3f)]
        private float m_fCircleErrorThreshold = 0.5f;

        private FixedSizeQueue<Vector3> points;
        private Vector3 lastPoint;
        private LineRenderer trail;

        private Vector3 debug_center = Vector3.zero;

        void Awake()
        {
            points = new FixedSizeQueue<Vector3>(m_iPointCount);
            trail = GetComponent<LineRenderer>();
        }

        void Start()
        {
            StartCoroutine("ConstantlyDequeue");
        }

        void Update()
        {
            TrackPoints();
            DetectCircle();
        }

        IEnumerator ConstantlyDequeue()
        {
            while (true)
            {
                if (points.Count > 0)
                    points.Dequeue();
                yield return new WaitForSeconds(m_fDequeueInterval);
            }
        }

        void DetectCircle()
        {
            if (points.Count < m_iMinimumIndexDiff)
                return;

            Vector3[] p = points.ToArray();
            var possibleIndices = new List<int>();

            // check if lastPoint is close enough to another point "crossing"
            for (int i=0; i<p.Length-1; ++i)
            {
                float distance = Vector2.Distance(p[i], lastPoint);
                if (distance < m_fMinimumDistance)
                {
                    possibleIndices.Add(i);
                }
            }

            if (possibleIndices.Count < 1) return;

            float radius = 0f;
            Vector3 center = Vector3.zero;
            bool circleFound = false;
            foreach (int index in possibleIndices)
            {
                // calc center of circle
                if (p.Length - index < m_iMinimumIndexDiff) return;
                center = Vector3.zero;
                for (int i = index; i < p.Length; ++i)
                {
                    center += p[i];
                }
                center /= p.Length - index;

                // calc radius  (point - center).Length() - radius
                radius = 0f;
                float[] radiusArr = new float[p.Length];
                for (int i = index; i < p.Length; ++i)
                {
                    radiusArr[i] = Vector3.Distance(p[i], center);
                    radius += radiusArr[i];
                }
                radius /= p.Length - index;

                // calc error
                float avgError = 0f;
                float radius100 = radius / 100f;
                for (int i = index; i < p.Length; ++i)
                {
                    float radiusDiff = Mathf.Abs(radius - radiusArr[i]);
                    avgError += radius100 * radiusDiff;
                }
                avgError /= p.Length - index;
                avgError /= radius;
                avgError *= 100f;

                // every radius must be roughly the same radius
                float minRadius = radius * (1 - m_fRadiusTolerance);
                float maxRadius = radius * (1 + m_fRadiusTolerance);
                circleFound = true;
                for (int i = index; i < p.Length; ++i)
                {
                    float cur = radiusArr[i];
                    if (cur > maxRadius ||
                        cur < minRadius)
                    {
                        //print("This is not a \"circle\"");
                        circleFound = false;
                        break;
                    }
                }

                if (circleFound)
                {
                    break;
                }
            }
            
            if (circleFound)
            {
                print("new circle found with radius=" + radius);
                debug_center = center;
            }

        }

        void TrackPoints()
        {
            // new point
            float x = Input.mousePosition.x;
            float y = Input.mousePosition.y;
            Vector3 p = Camera.main.ScreenToWorldPoint(new Vector3(x, y, 10));

            // has the mouse moved significantly enough?
            if (Vector3.Distance(p, lastPoint) > m_fMinimumDistance)
            {
                lastPoint = p;
                points.Enqueue(p);
            }
            // if not, throw another point away as well
            else
            {
                //if (points.Count > 0)
                //    points.Dequeue();
            }

            if (trail != null)
            {
                trail.positionCount = points.Count;
                trail.SetPositions(points.ToArray());
            }
        }

        void OnDrawGizmos()
        {
            Gizmos.color = new Color(1, 0, 1);
            Gizmos.DrawCube(debug_center, Vector3.one * 0.5f);
        }
    }
}
