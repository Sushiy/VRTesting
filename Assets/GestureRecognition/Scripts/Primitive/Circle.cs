using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace primitive
{
    [RequireComponent(typeof(LineRenderer))]
    public class Circle : MonoBehaviour {

        [SerializeField]
        private int m_iNumPoints = 64;

        private float radius = 3f;
        private LineRenderer m_Line;

        void Awake()
        {
            m_Line = GetComponent<LineRenderer>();
            m_Line.positionCount = m_iNumPoints;

            Assert.IsNotNull<LineRenderer>(m_Line);
        }

        void Start()
        {
            CalculateNewPoints();
        }

        void CalculateNewPoints()
        {
            Vector3 center = transform.position;
            Vector3[] points = new Vector3[m_iNumPoints];

            for (int i=0; i < m_iNumPoints; ++i)
            {
                float deg = ((2 * Mathf.PI)/65f) * i;
                points[i] = center + new Vector3(Mathf.Cos(deg), Mathf.Sin(deg), 0) * radius;
            }

            m_Line.SetPositions(points);
        }
	
	    public void setRadius(float radius)
        {
            this.radius = radius;
            CalculateNewPoints();
        }
    }
}
