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
    [SerializeField]
    private float m_fRayLength = 20f;
    [SerializeField]
    private bool m_bEnableRaycast = true;
    [SerializeField]
    private float m_fDuration = 10f;

    private Vector3 m_v3target;
    private LineRenderer m_lineRenderer;
    private Rigidbody m_rigidTargetSphere;
    private Transform m_transPhysicsSphere;
    private float m_fDurationTimer = 0f;
    private bool m_bIsEnding = false;
    private Transform m_transSparks;

    public override void Awake()
    {
        base.Awake();

        m_lineRenderer = GetComponent<LineRenderer>();
        m_lineRenderer.positionCount = m_iNumPoints;
        m_rigidTargetSphere = transform.Find("TargetSphere").GetComponent<Rigidbody>();
        m_transPhysicsSphere = transform.Find("PhysicsSphere").GetComponent<Transform>();
        Assert.IsNotNull(m_rigidTargetSphere);
        Assert.IsNotNull(m_transPhysicsSphere);
        m_transSparks = transform.Find("EndSparks");
        Assert.IsNotNull(m_transSparks);
    }

    public override void Update()
    {
        base.Update();

        // update the particle effects on the tip
        m_transSparks.position = getEndPosition();
        m_transSparks.LookAt(transform.position);

        // kill the spell
        if (!m_bIsEnding)
        {
            m_fDurationTimer += Time.deltaTime;
            if (m_fDurationTimer >= m_fDuration)
            {
                EndSpell();
            }
        }
    }


    void FixedUpdate () {
        if (m_bEnableRaycast)
        {
            RaycastHit hit;
            if (Physics.Raycast(new Ray(transform.position, transform.forward), out hit, 100f, m_layerMask)){
                m_v3target = hit.point;
            }
            else
            {
                m_v3target = transform.position + transform.forward * m_fRayLength;
            }
        }
        else
            m_v3target = transform.position + transform.forward * m_fRayLength;

        m_rigidTargetSphere.MovePosition(m_v3target);
        CalculateLinePoints();
    }

    void CalculateLinePoints()
    {
        // BEZIER CURVE
        Vector3[] bezPoints = new Vector3[3];
        bezPoints[0] = transform.position;
        bezPoints[1] = transform.position + transform.forward;
        //bezPoints[1] = Vector3.Project(m_v3target, transform.forward) * 0.5f;
        bezPoints[2] = m_transPhysicsSphere.transform.position;

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
    }

    public override void Fire(CastingData spelldata)
    {
        Debug.Log("LaserBeam: Fire");
        //Fetch the MPVRPlayerController
        IPlayerController player = spelldata._goPlayer.GetComponent<IPlayerController>();
        //Get the transform of the currently casting hand
        Transform transCastingHand = player.GetCastingHand(spelldata._iCastingHandIndex);
        //Get rigidbody and the fixedjoint of the casting hand
        Rigidbody rigidSpell = GetComponent<Rigidbody>();
        transform.position = spelldata._v3WandPos;
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

    void EndSpell()
    {
        Destroy(m_transPhysicsSphere.gameObject);
        Destroy(this.gameObject);
    }

    public Vector3 getEndPosition()
    {
        return m_lineRenderer.GetPosition(m_iNumPoints - 1);
    }
}
