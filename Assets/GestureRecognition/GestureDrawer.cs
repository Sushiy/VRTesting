using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace gesture
{
    /// <summary>
    /// Records the raw gesture material (points array)
    /// and transforms it into data (characteristic points)
    /// that can be further processed.
    /// The characteristic points will be matched with
    /// the gesture data.
    /// </summary>
    [RequireComponent(typeof(LineRenderer))]
    public class GestureDrawer : MonoBehaviour
    {
        [SerializeField]
        private float m_fSphereRadius = 0.3f;
        [SerializeField]
        [Range(0, 360)]
        private int m_iMaxAngle = 20;
        [SerializeField]
        [Range(0f, 2f)]
        private float m_fMinimalDistance = 0.2f;
        [SerializeField]
        [Range(1, 10)]
        private int m_iRequiredNumber = 5;
        [SerializeField]
        private float m_fSize = 1f;

        // Debug Texts
        [SerializeField]
        private Text m_textCharPntNr;

        private List<Vector3> m_arrPoints = new List<Vector3>(); // just for the line renderer
        private List<Vector2> m_arrPoints2D = new List<Vector2>();
        private List<Vector2> m_arrCharPoints = new List<Vector2>();
        private List<Vector2> m_arrAddedPoints = new List<Vector2>(); // debug
        private List<Vector2> m_arrRemovedPoints = new List<Vector2>(); // debug

        private LineRenderer line;
        private bool newLine = true;

        void Awake()
        {
            line = GetComponent<LineRenderer>();
        }

        /// <summary>
        /// Drawing and recording gestures!
        /// </summary>
        void Update()
        {
            Debug();

            if (Input.GetMouseButton(0))
            {
                if (newLine)
                {
                    m_arrPoints.Clear();
                    m_arrPoints2D.Clear();
                    m_arrCharPoints.Clear();
                    newLine = false;
                }

                // Record current point
                float x = Input.mousePosition.x;
                float y = Input.mousePosition.y;
                Vector3 p = Camera.main.ScreenToWorldPoint(new Vector3(x, y, 10));
                m_arrPoints.Add(p);
                m_arrPoints2D.Add(p);

                // draw the line
                line.SetVertexCount(m_arrPoints.Count);
                line.SetPositions(m_arrPoints.ToArray());
            }

            if (Input.GetMouseButtonUp(0))
            {
                // Calculate characteristic points of gesture
                Vector2[] charp;
                Vector2[] p = m_arrPoints2D.ToArray();
                IdentifyCharPoints(ref p, out charp, m_iRequiredNumber);
                // Make it Uniform
                MakeGestureUniform(ref charp, 1f);
                m_arrCharPoints.AddRange(charp);
                newLine = true;
            }
        }

        public gesture CreateGestureFromViveData(ref Vector3[] p, ref Vector3 lookAt)
        {
            gesture g = new gesture();
            Vector2[] points2D;
            Transform3DData(ref p, ref lookAt, out points2D);
            IdentifyCharPoints(ref points2D, out g.points, m_iRequiredNumber);
            MakeGestureUniform(ref g.points, m_fSize);
            return g;
        }

        public Vector2[] getCurrentGesture()
        {
            return m_arrCharPoints.ToArray();
        }

        void Debug()
        {
            if (m_textCharPntNr != null)
            {
                m_textCharPntNr.text = "CharPoints: " + m_arrCharPoints.Count;
            }
        }

        void Transform3DData(ref Vector3[] p, ref Vector3 lookAt, out Vector2[] gesturePoints)
        {
            ProjectOnPlane(ref p, ref lookAt);
            RotateToXYPlane(ref p, ref lookAt);
            gesturePoints = new Vector2[p.Length];
            for (int i=0; i<p.Length; ++i)
            {
                gesturePoints[i] = p[i];
            }
        }

        void ProjectOnPlane(ref Vector3[] p, ref Vector3 normal)
        {
            for (int i=0; i<p.Length; ++i)
            {
                float dist = Vector3.Dot(p[i], normal);
                p[i] = p[i] - dist * normal;
            }
        }

        void RotateToXYPlane(ref Vector3[] p, ref Vector3 normal)
        {
            Quaternion rotation = Quaternion.FromToRotation(normal, Vector3.forward);
            for (int i=0; i<p.Length; ++i)
            {
                p[i] = rotation * p[i];
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

            for (int i = 1; i < m_arrPoints.Count - 1; ++i)
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
                else
                    result.Add(lastPoint);
            }

            m_arrAddedPoints.Clear(); // DEBUG LINE
            m_arrRemovedPoints.Clear(); // DEBUG LINE

            /* Step 2: Increase points? */
            if (result.Count < requiredNr)
            {
                int diff = requiredNr - result.Count;
                AddPointsToLongestSegments(ref result, diff);
            }
            /* Step 3: Decrease points? */
            else if (result.Count > requiredNr)
            {
                int diff = result.Count - requiredNr;
                RemovePointsFromGesture(ref result, diff);
            }

            // Last Step: Assign the result
            charp = result.ToArray();
        }

        /// <summary>
        /// iteratively remove points from Result with the shortest sum
        /// of the segments, but preserve the points: start and end
        /// </summary>
        /// <param name="p"></param>
        /// <param name="number"></param>
        void RemovePointsFromGesture(ref List<Vector2> p, int number)
        {
            /**
             * There could be a better way to remove them,
             * because with algorithm does not take a angle in account.
             * If it makes trouble, this might be a good start
             **/

            // Check for traps
            if (p.Count <= 2)
            {
                print("Algorithm needs at least 2 segments to reduce points.");
                return;
            }

            for (int n = 0; n < number; ++n)
            {
                // init
                float minSumOfDistance = float.MaxValue;
                int index = -1;

                // find the point with the shortest sum of segment-lengths
                for (int i = 1; i < p.Count - 1; ++i)
                {
                    float sum = Vector2.Distance(p[i - 1], p[i]); // distance of the first adjacent segment
                    sum += Vector2.Distance(p[i], p[i + 1]); // distance of the second adjacet segment
                    if (sum < minSumOfDistance)
                    {
                        minSumOfDistance = sum;
                        index = i;
                    }
                }

                m_arrRemovedPoints.Add(p[index]); // DEBUG

                p.RemoveAt(index);
            }
        }

        /// <summary>
        /// Iteratively adds points to the longest segmets
        /// </summary>
        /// <param name="p">The point list where points will be added</param>
        /// <param name="number">The number of points that shall be added</param>
        void AddPointsToLongestSegments(ref List<Vector2> p, int number)
        {
            /**
             * This could also be done better.
             * Add points by using the "velocity"/tangents of the segments
             * before and after (just like Catmull Rom or alike)
             **/

            // Check for traps
            if (p.Count <= 1)
            {
                print("Not a single segment to add points to! Please record more points!");
                return;
            }

            // Add the points
            for (int n = 0; n < number; ++n)
            {
                // Init for a point
                float maxDistance = float.MinValue;
                int segmentStart = -1;

                // find the longest distance
                for (int i = 0; i < p.Count - 1; ++i)
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

                m_arrAddedPoints.Add(newPoint); // DEBUG LINE

                p.Insert(segmentStart + 1, newPoint);
            }
        }

        /// <summary>
        /// Centers the gesture to (0,0) and
        /// Equally scales the gesture up to a size
        /// </summary>
        /// <param name="p"></param>
        /// <param name="size"></param>
        void MakeGestureUniform(ref Vector2[] p, float size)
        {
            Vector2 min = new Vector2(float.MaxValue, float.MaxValue);
            Vector2 max = new Vector2(float.MinValue, float.MinValue);

            // find min and max of the bounding box of this gesture
            for (int i = 0; i < p.Length; ++i)
            {
                if (p[i].x < min.x)
                    min.x = p[i].x;
                if (p[i].x > max.x)
                    max.x = p[i].x;
                if (p[i].y < min.y)
                    min.y = p[i].y;
                if (p[i].y > max.y)
                    max.y = p[i].y;
            }

            // scale to the right size
            Rect box = new Rect(min.x, min.y, max.x - min.x, max.y - min.y);
            float scaleX = size / box.width;
            float scaleY = size / box.height;
            Vector2 center = new Vector2(box.center.x * scaleX, box.center.y* scaleY);
            for (int i = 0; i < p.Length; ++i)
            {
                // scale
                p[i].x *= scaleX;
                p[i].y *= scaleY;
                // translate
                p[i].x -= center.x;
                p[i].y -= center.y;
            }
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
                Gizmos.DrawSphere(new Vector3(p.x, p.y, 0), m_fSphereRadius/3f);
                r += colorStep;
                g += colorStep;
                b += colorStep;
            }

            // Mark the Space where the gesture should get drawn then
            Gizmos.color = Color.white;
            Gizmos.DrawLine(Vector2.zero + 0.5f * Vector2.up - 0.5f * Vector2.left,
                Vector2.zero + 0.5f * Vector2.up - 0.5f * Vector2.right);
            Gizmos.DrawLine(Vector2.zero + 0.5f * Vector2.down - 0.5f * Vector2.left,
                Vector2.zero + 0.5f * Vector2.down - 0.5f * Vector2.right);
            Gizmos.DrawLine(Vector2.zero + 0.5f * Vector2.up - 0.5f * Vector2.left,
                Vector2.zero + 0.5f * Vector2.down - 0.5f * Vector2.left);
            Gizmos.DrawLine(Vector2.zero + 0.5f * Vector2.up - 0.5f * Vector2.right,
                Vector2.zero + 0.5f * Vector2.down - 0.5f * Vector2.right);


        }
    }

}