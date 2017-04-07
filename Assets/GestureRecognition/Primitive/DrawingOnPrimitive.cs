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
        private FixedSizeQueue<Vector3> debug_points;
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

            debug_points = new FixedSizeQueue<Vector3>(m_iNumberOfPoints);

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
                    debug_points.Enqueue(hit.point);
                    m_v3LastPoint = hit.point;

                    LineRenderer line = c.gameObject.GetComponent<LineRenderer>();
                    if (line != null)
                    {
                        line.positionCount = debug_points.Count;
                        line.SetPositions(debug_points.ToArray());
                    }
                }
            }
            else
            {
                print("there is something wrong. the raycast didnt hit. whats up?");
            }
        }

        void OnTriggerExit(Collider c)
        {
            //// create and match the gestures
            Vector3[] rawPoints = debug_points.ToArray();
            Vector2[] points2D = new Vector2[rawPoints.Length];
            Matrix4x4 matrix = c.transform.worldToLocalMatrix;
            for (int i = 0; i < rawPoints.Length; ++i)
            {
                Vector3 localPoint = matrix * rawPoints[i];
                points2D[i] = new Vector2(localPoint.x, -localPoint.z);
                print(points2D[i]);
            }
            GestureObject g = m_converter.CreateGestureFrom2DData(ref points2D);
            print("gesturepoints: " + g.points.Length);

            /*
             * next steps:
             * draw the 2d lines in 3d space to debug stuff
             * why is the line stuff not working at all!?
             * debug all the things ToT
             */

            //Vector3 normal = c.transform.up;
            //GestureObject g = m_converter.CreateGestureFrom3DDataFromPrimitive(ref rawPoints, ref normal);
            //print("gesturepoints " + g.points.Length);
            gestureTypes type;
            bool valid = m_matcher.Match(g, out type);
            print("This gesture is: " + type.ToString() + " and it is valid: " + valid);

            // clear the lines
            debug_points.Clear();
            LineRenderer line = c.gameObject.GetComponent<LineRenderer>();
            if (line != null)
            {
                line.positionCount = 0;
            }
        }
    }
}
