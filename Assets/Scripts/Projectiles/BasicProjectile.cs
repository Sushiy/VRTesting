using UnityEngine;
using System.Collections;

public class BasicProjectile : Projectile
{
    public ParticleSystem m_PSExplosion;

    public MeshRenderer m_meshRendererThis;
    public float m_fShotPower = 15f;
    private Rigidbody m_rigidThis;

    void Start()
    {
        m_rigidThis = GetComponent<Rigidbody>();
    }
    public override void Fire()
    {
        base.Fire();
        //print("firebasic!");
        if(m_rigidThis == null)
            m_rigidThis = GetComponent<Rigidbody>();
        m_rigidThis.AddForce(transform.forward * m_fShotPower, ForceMode.Impulse);
    }

    public override float GetRange()
    {
        return base.GetRange();
    }

    public override void DestroyThis()
    {
        base.DestroyThis();
        ExplodeAfter(0.0f);
    }



    public IEnumerator ExplodeAfter(float f)
    {
        yield return new WaitForSeconds(f);
        m_PSExplosion.Play();
        m_meshRendererThis.gameObject.SetActive(false);
        while (m_PSExplosion.IsAlive())
        {
            yield return null;

        }

        Destroy(this.gameObject);
    }
}
