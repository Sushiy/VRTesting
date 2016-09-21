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
        m_rigidThis.AddForce(m_transWand.forward * m_fShotPower, ForceMode.Impulse);
    }
}
