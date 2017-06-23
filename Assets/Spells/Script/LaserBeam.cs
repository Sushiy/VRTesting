using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(LineRenderer))]
public class LaserBeam : MonoBehaviour {

    [SerializeField]
    private LayerMask m_layerMask;
    [SerializeField]
    private int m_iNumPoints = 10;

    private Vector3 m_v3target;
    private LineRenderer m_lineRenderer;
    private Rigidbody targetSphere;
    private Transform physicsSphere;
    private Vector3[] bezierPoints;
	
    void Awake()
    {
        m_lineRenderer = GetComponent<LineRenderer>();
        m_lineRenderer.positionCount = m_iNumPoints;
        targetSphere = transform.Find("TargetSphere").GetComponent<Rigidbody>();
        physicsSphere = transform.Find("PhysicsSphere").GetComponent<Transform>();
        Assert.IsNotNull(targetSphere);
        Assert.IsNotNull(physicsSphere);

        bezierPoints = new Vector3[m_iNumPoints];
    }

	void FixedUpdate () {
        RaycastHit hit;
        if (Physics.Raycast(new Ray(transform.position, transform.forward), out hit, 100f, m_layerMask)){
            m_v3target = hit.point;
        }
        else
        {
            m_v3target = transform.position + transform.forward * 30f;
        }

        targetSphere.MovePosition(m_v3target);
        CalculateLinePoints();
    }

    void CalculateLinePoints()
    {
        // BEZIER CURVE
        Vector3[] bezPoints = new Vector3[3];
        bezPoints[0] = transform.position;
        bezPoints[1] = Vector3.Project(m_v3target, transform.forward) / 2f;
        bezPoints[2] = physicsSphere.transform.position;

        for (int i = 0; i < m_iNumPoints; ++i)
        {
            float t = i / ((float)m_iNumPoints - 1f);

            for (int j = bezPoints.Length; j >= 1; j--)
            {
                for (int k = 0; k < j - 1; k++)
                {
                    bezPoints[k] = Vector3.Lerp(bezPoints[k], bezPoints[k + 1], t);
                }
            }

            m_lineRenderer.SetPosition(i, bezPoints[0]);
        }

        // SIMPLE LINE
        //for (int i = 0; i < m_iNumPoints; ++i)
        //{
        //    Vector3 pos = Vector3.Lerp(transform.position, m_v3target, i / ((float)m_iNumPoints-1f));
        //    m_lineRenderer.SetPosition(i, pos);
        //}
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + transform.forward*30f);    
    }
}
