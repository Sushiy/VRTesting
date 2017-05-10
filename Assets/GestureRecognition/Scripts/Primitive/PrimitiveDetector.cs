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
    /// TODO detect edges and count them
    /// TODO check if edges "overlapped" and see if the thing is big enough
    /// </summary>
    [RequireComponent(typeof(LineRenderer))]
    public class PrimitiveDetector : MonoBehaviour
    {
        public LineRenderer debug_lineRenderer;

        [SerializeField]
        private float m_fDequeueInterval = 0.5f; // how fast are points dequeued again
        [SerializeField]
        private int m_iPointCount = 256; // how many points can the queue hold?
        [SerializeField]
        private float m_fMinimumDistance = 0.25f; // minimum distance between recorded points
        [SerializeField]
        private float m_fMinimumDistanceCrossing = 0.25f; // minimum distance between points to be considered "crossing"
        [SerializeField]
        private int m_iMinimumIndexDiff = 40; // minimum number of points of shapes (length)
        [SerializeField]
        [Range(0f, 1f)]
        private float m_fRadiusTolerance = 0.25f; // minimum radius tolerance of a circle shape
        [SerializeField]
        private bool m_trackVR = false;
        [SerializeField]
        private GameObject prefab_Circle;

        private FixedSizeQueue<Vector3> points;
        private Vector3 lastPoint;
        private LineRenderer trail;
        private Transform m_transform;

        private int debugCount = 0; //debug
        public TextMesh debug_text; //debug

        //private Vector3 debug_center = Vector3.zero;

        void Awake()
        {
            points = new FixedSizeQueue<Vector3>(m_iPointCount);
            trail = GetComponent<LineRenderer>();
            Assert.IsNotNull<LineRenderer>(trail);
            m_transform = GetComponent<Transform>();
            Assert.IsNotNull<GameObject>(prefab_Circle);
        }

        void Start()
        {
            StartCoroutine("ConstantlyDequeue");
        }

        void Update()
        {
            // only if no primitive is in the scene, there are new primitives to detect!
            if (Primitive.PrimitiveCount == 0)
            {
                TrackPoints();
                DetectCircle();
            }
            else
            {
                if (points.Count > 0)
                    points.Dequeue();
            }
        }

        /// <summary>
        /// Constantly dequeues points to make the trail "vanish"
        /// </summary>
        /// <returns></returns>
        IEnumerator ConstantlyDequeue()
        {
            while (true)
            {
                if (points.Count > 0)
                    points.Dequeue();
                yield return new WaitForSeconds(m_fDequeueInterval);
            }
        }

        /// <summary>
        /// Tries to detect a circle in the queue
        /// </summary>
        void DetectCircle()
        {
            // is the shape/circle "long enough" ?
            if (points.Count < m_iMinimumIndexDiff)
                return;

            Vector3[] p = points.ToArray();
            var possibleIndices = new List<int>();

            // check if lastPoint is close enough to another point to detect "crossing"
            for (int i = 0; i < p.Length - m_iMinimumIndexDiff; ++i)
            {
                float distance = Vector2.Distance(p[i], lastPoint);
                if (distance < m_fMinimumDistanceCrossing)
                {
                    possibleIndices.Add(i);
                }
            }

            // no crossings detected. end here.
            if (possibleIndices.Count < 1) return;

            // for every possible circle do the following:
            float radius = 0f;
            Vector3 center = Vector3.zero;
            bool circleFound = false;
            Vector3 firstPoint = Vector3.zero, quarterPoint = Vector3.zero;
            foreach (int index in possibleIndices)
            {
                // is the circle big enough?
                if (p.Length - index < m_iMinimumIndexDiff) return;

                // calc center of circle
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
                //float avgError = 0f;
                //float radius100 = radius / 100f;
                //for (int i = index; i < p.Length; ++i)
                //{
                //    float radiusDiff = Mathf.Abs(radius - radiusArr[i]);
                //    avgError += radius100 * radiusDiff;
                //}
                //avgError /= p.Length - index;
                //avgError /= radius;
                //avgError *= 100f;

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
                        circleFound = false;
                        break;
                    }
                }

                if (circleFound)
                {
                    // debug ausgabe
                    if (debug_lineRenderer != null)
                    {
                        print("Detected a circle. Will debug draw now.");
                        debug_lineRenderer.positionCount = p.Length - index;
                        for (int j = index; j < debug_lineRenderer.positionCount; ++j)
                        {
                            debug_lineRenderer.SetPosition(j, p[j]);
                        }
                        print("Ended Drawing the Circle");
                    }
                    // debug end

                    firstPoint = p[index];
                    quarterPoint = p[index + (p.Length - index) / 3];
                    break;
                }
            }

            if (circleFound)
            {
                if (debug_text != null) debug_text.text = "Circles found: " + debugCount++;
                //print("new circle found with radius=" + radius);
                InstantiateCircle(firstPoint, quarterPoint, center, radius);
            }
        }

        void InstantiateCircle(Vector3 firstPoint, Vector3 quarterPoint, Vector3 center, float radius)
        {
            // calculate normal
            Vector3 normal = Vector3.Cross(firstPoint - center, quarterPoint - center).normalized;

            // instantiate circle
            GameObject circle = Instantiate<GameObject>(prefab_Circle);
            Primitive primitive = circle.GetComponent<Primitive>();

            // check for traps
            if (primitive == null)
            {
                Debug.LogError("When instantiating this circle, there was no Primitive Component attached!");
            }

            // try to find out if circle is clockwise or counterclockwise
            bool clockwise = isClockwise(center, firstPoint, quarterPoint);

            // set position
            float turnAround = (clockwise) ? -1f : 1f;
            primitive.setPosition(turnAround * normal, center, radius);
        }

        /*
         * A                 
           |\     // A = Rotation Center
           | \    // B = Previous Frame Position
           |  C   // C = Current Frame Position
           B
         */
        bool isClockwise(Vector2 center, Vector2 firstPoint, Vector2 quarterPoint)
        {
            return ((firstPoint.x - center.x) * (quarterPoint.y - center.y) - (firstPoint.y - center.y) * (quarterPoint.x - center.x)) > 0;
        }

        /// <summary>
        /// Tracks the mouse and stores the points in to the queue.
        /// 
        /// TODO make it able to track a wand as well!
        /// </summary>
        void TrackPoints()
        {
            // new point
            Vector3 p;
            if (m_trackVR)
            {
                p = m_transform.position;
            }
            else
            {
                float x = Input.mousePosition.x;
                float y = Input.mousePosition.y;
                p = Camera.main.ScreenToWorldPoint(new Vector3(x, y, 10));
            }

            // has the mouse moved significantly enough?
            if (Vector3.Distance(p, lastPoint) > m_fMinimumDistance)
            {
                // store the point
                lastPoint = p;
                points.Enqueue(p);
            }

            // draw the trail
            if (trail != null)
            {
                trail.positionCount = points.Count;
                trail.SetPositions(points.ToArray());
            }
        }
    }
}
