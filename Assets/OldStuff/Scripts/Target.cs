using UnityEngine;
using System.Collections;

public class Target : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem m_psExplosion;
    public int Lifes = 3;
    public bool DestroyMe = false;
	public void OnCollisionEnter(Collision _coll)
    {
        if (_coll.gameObject.layer == 9)
        {
            StartCoroutine(ExplodeAfter(0.0f));
        }
    }

    public IEnumerator ExplodeAfter(float f)
    {
        yield return new WaitForSeconds(f);
        m_psExplosion.Play();
        while (m_psExplosion.IsAlive())
        {
            yield return null;

        }
        if(DestroyMe)
        {
            Lifes--;
            if(Lifes <= 0)
                Destroy(this.gameObject);
        }
    }
}
