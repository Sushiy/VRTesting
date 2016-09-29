using UnityEngine;
using System.Collections;

public class StickyProjectileScript : MonoBehaviour
{
    public ParticleSystem m_PSExplosion;
    public float boomDelay = 2.0f;

    public MeshRenderer m_meshRendererThis;
    public LayerMask m_layermask;

    public void OnCollisionEnter(Collision _coll)
    {
        if( m_layermask == (m_layermask | (1 << _coll.gameObject.layer)))
        {
            if(_coll.gameObject.layer != 10)
                GetComponent<Rigidbody>().isKinematic = true;
            StartCoroutine(ExplodeAfter(boomDelay));
        }
    }

    public IEnumerator ExplodeAfter(float f)
    {
        yield return new WaitForSeconds(f);
        m_PSExplosion.Play();
        m_meshRendererThis.gameObject.SetActive(false);
        while(m_PSExplosion.IsAlive())
        {
            yield return null;

        }

        Destroy(this.gameObject);
    }
}
