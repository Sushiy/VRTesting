using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using gestureUtil;

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

        void Awake()
        {
            m_transform = transform;

            m_transBeginning = m_transform.FindChild("beginningPoint");
            Assert.IsNotNull<Transform>(m_transBeginning);

            m_transEnd = m_transform.FindChild("endPoint");
            Assert.IsNotNull<Transform>(m_transEnd);

            debug_points = new FixedSizeQueue<Vector3>(m_iNumberOfPoints);

            m_v3LastPoint = Vector3.zero;
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
            debug_points.Clear();
            LineRenderer line = c.gameObject.GetComponent<LineRenderer>();
            if (line != null)
            {
                line.positionCount = 0;
            }
        }
    }
}
