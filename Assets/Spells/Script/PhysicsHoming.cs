using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PhysicsHoming : MonoBehaviour {

    [SerializeField]
    private float m_fTorqueForce = 0.1f;
    [SerializeField]
    private float m_fConstantForce = 0.1f;

    private Rigidbody m_rigid;
    private Transform m_transTarget;

    void Awake()
    {
        m_rigid = GetComponent<Rigidbody>();
    }

	void Start () {
        // find a target
        m_transTarget = GameObject.Find("DebugTarget").transform;
        if (m_transTarget == null) Debug.LogWarning("Couldn't find no target y'all");
    }

    void FixedUpdate () {
        if (m_transTarget == null) return;

        Vector3 targetDelta = m_transTarget.position - transform.position;

        //get the angle between transform.forward and target delta
        float angleDiff = Vector3.Angle(transform.forward, targetDelta);

        // get its cross product, which is the axis of rotation to
        // get from one vector to the other
        Vector3 cross = Vector3.Cross(transform.forward, targetDelta);

        // apply torque along that axis according to the magnitude of the angle.
        m_rigid.AddTorque(cross * angleDiff * m_fTorqueForce);
    }
}
