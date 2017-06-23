using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(LineRenderer))]
public class LaserBeam : Spell {

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
        bezPoints[1] = Vector3.Project(m_v3target, transform.forward) * 0.25f;
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

    public override void Fire(CastingData spelldata)
    {
        Debug.Log("LaserBeam: Fire");
        //Fetch the MPVRPlayerController
        MP_VR_PlayerController player = spelldata._goPlayer.GetComponent<MP_VR_PlayerController>();
        //Get the transform of the currently casting hand
        Transform transCastingHand = player.GetCastingHand(spelldata._iCastingHandIndex);
        //Get rigidbody and the fixedjoint of the casting hand
        Rigidbody rigidSpell = GetComponent<Rigidbody>();
        transform.position = transCastingHand.position;
        transform.rotation = transCastingHand.rotation;
        FixedJoint fixJOffhand = transCastingHand.GetComponent<FixedJoint>();
        if (fixJOffhand.connectedBody != null)
            Destroy(fixJOffhand.connectedBody.gameObject);
        fixJOffhand.connectedBody = rigidSpell;
    }

    public override void PlayerHit(GameObject _goPlayer)
    {
        throw new NotImplementedException();
    }

    public override void SpellHit()
    {
        throw new NotImplementedException();
    }

    public override void Deactivate()
    {
        throw new NotImplementedException();
    }
}
