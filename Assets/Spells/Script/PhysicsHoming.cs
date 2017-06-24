using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PhysicsHoming : MonoBehaviour {

    [SerializeField]
    private float m_fTorqueForce = 0.1f;
    [SerializeField]
    //private float m_fConstantForce = 0.1f;

    private Rigidbody m_rigid;
    private Transform m_transTarget;

    void Awake()
    {
        m_rigid = GetComponent<Rigidbody>();

    }

	void Start ()
    {
        // find a target
        m_transTarget = GetComponent<Spell>().TargetTransform;
        if (m_transTarget == null) Debug.LogWarning("Couldn't find no target y'all");
    }

    void FixedUpdate () {
        Vector3 v3TargetPos = Vector3.zero;
        if (m_transTarget == null) Debug.LogWarning("No Target, Firing at 0,0,0");
        else v3TargetPos = m_transTarget.position;

        Vector3 targetDelta = v3TargetPos + new Vector3(0, 0, 0) - transform.position;

        //get the angle between transform.forward and target delta
        float angleDiff = Vector3.Angle(transform.forward, targetDelta);

        // get its cross product, which is the axis of rotation to
        // get from one vector to the other
        Vector3 cross = Vector3.Cross(transform.forward, targetDelta);

        // apply torque along that axis according to the magnitude of the angle.
        m_rigid.AddTorque(cross * angleDiff * m_fTorqueForce);
    }
}
