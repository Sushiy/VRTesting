using UnityEngine;
using System.Collections;

public class BasicProjectile : Projectiles
{
    public float m_fShotPower = 15f;
    private Rigidbody m_rigidThis;

    void Start()
    {
        m_rigidThis = GetComponent<Rigidbody>();
    }
    public override void Fire()
    {
        base.Fire();
        print("firebasic!");
        if(m_rigidThis == null)
            m_rigidThis = GetComponent<Rigidbody>();
        m_rigidThis.AddForce(transform.forward * m_fShotPower, ForceMode.Impulse);
    }

    public override float GetRange()
    {
        return base.GetRange();
    }
}
