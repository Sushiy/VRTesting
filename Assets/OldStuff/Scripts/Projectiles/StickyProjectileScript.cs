using UnityEngine;
using System.Collections;

public class StickyProjectileScript : MonoBehaviour
{
    public LayerMask m_layermask;

    public void OnCollisionEnter(Collision _coll)
    {
        if (m_layermask == (m_layermask | (1 << _coll.gameObject.layer)))
        {
            if (_coll.gameObject.layer != 10)
            {
                GetComponent<Rigidbody>().isKinematic = true;
                StartCoroutine(ExplodeAfter(1.5f));
            }
        }

    }


    public IEnumerator ExplodeAfter(float f)
    {
        yield return new WaitForSeconds(f);
        GetComponent<Projectile>().DestroyThis();
    }
}
