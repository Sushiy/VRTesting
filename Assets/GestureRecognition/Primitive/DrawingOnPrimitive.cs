using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using gestureUtil;
using gesture;

namespace primitive
{
    public class DrawingOnPrimitive : MonoBehaviour
    {
        [SerializeField]
        private int m_iNumberOfPoints = 1024;
        [SerializeField]
        private float m_fMinimumDistance = 0.3f;

        private Transform m_transform;
        private Transform m_transBeginning;
        private Transform m_transEnd;
        private FixedSizeQueue<Vector3> points;
        private Vector3 m_v3LastPoint;

        private GestureConverter m_converter;
        private GestureMatcher m_matcher;

        void Awake()
        {
            m_transform = transform;

            m_transBeginning = m_transform.FindChild("beginningPoint");
            Assert.IsNotNull<Transform>(m_transBeginning);

            m_transEnd = m_transform.FindChild("endPoint");
            Assert.IsNotNull<Transform>(m_transEnd);

            points = new FixedSizeQueue<Vector3>(m_iNumberOfPoints);

            m_v3LastPoint = Vector3.zero;

            m_converter = GameObject.FindGameObjectWithTag("GestureObject").GetComponent<GestureConverter>();
            Assert.IsNotNull<GestureConverter>(m_converter);

            m_matcher = m_converter.GetComponent<GestureMatcher>();
            Assert.IsNotNull<GestureMatcher>(m_matcher);
        }

        void OnTriggerEnter(Collider c)
        {
            OnTriggerStay(c);
        }

        void OnTriggerStay(Collider c)
        {
            // only for primitives
            if (!c.CompareTag("Primitive"))
                return;

            // calculate the hitting point
            RaycastHit hit;
            Vector3 wand = m_transEnd.position - m_transBeginning.position;
            if (c.Raycast(new Ray(m_transBeginning.position, (m_transEnd.position - m_transBeginning.position)),
                out hit,
                wand.magnitude))
            {
                if (Vector3.Distance(hit.point, m_v3LastPoint) > m_fMinimumDistance)
                {
                    // add to the debug queue
                    points.Enqueue(hit.point);
                    m_v3LastPoint = hit.point;

                    LineRenderer line = c.gameObject.GetComponent<LineRenderer>();
                    if (line != null)
                    {
                        line.positionCount = points.Count;
                        line.SetPositions(points.ToArray());
                    }
                }
            }
        }

        void OnTriggerExit(Collider c)
        {
            // convert 3D points to 2D points on the plane
            Vector3[] rawPoints = points.ToArray();
            Vector2[] points2D = new Vector2[rawPoints.Length];
            Matrix4x4 matrix = c.transform.worldToLocalMatrix;
            for (int i = 0; i < rawPoints.Length; ++i)
            {
                Vector3 localPoint = matrix * rawPoints[i];
                points2D[i] = new Vector2(localPoint.x, localPoint.z);
            }

            // convert 2D points to a gesture
            GestureObject g = m_converter.CreateGestureFrom2DData(ref points2D);

            // match the gesture
            gestureTypes type;
            bool valid = m_matcher.Match(g, out type);
            print("This gesture is: " + type.ToString() + " and it is valid: " + valid); //debug

            // clear the lines
            points.Clear();
            LineRenderer line = c.gameObject.GetComponent<LineRenderer>();
            if (line != null)
            {
                line.positionCount = 0;
            }
        }
    }
}
