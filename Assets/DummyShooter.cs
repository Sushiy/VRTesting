using UnityEngine;
using System.Collections;

public class DummyShooter : MonoBehaviour
{
    private GameObject projectile;
    public GameObject projectileFire;
    public GameObject projectileIce;

    public ParticleSystem m_psFire;
    public ParticleSystem m_psIce;

    Transform target;

	// Use this for initialization
	void Start ()
    {
        target = GameObject.FindGameObjectWithTag("MainCamera").transform;
        StartCoroutine(ShootMe());
	}

    IEnumerator ShootMe()
    {
        while(true)
        {
            yield return new WaitForSeconds(Random.Range(3.0f, 8.0f));
            ChooseAttack();
            yield return new WaitForSeconds(Random.Range(1.0f, 3.0f));
            Fire();
        }
    }

    void ChooseAttack()
    {
        float i = Random.Range(0.0f, 1.0f);
        if (i > 0.5f)
        {
            projectile = projectileFire;
            m_psFire.Play();
            m_psIce.Stop();
        }
        else
        {
            projectile = projectileIce;
            m_psIce.Play();
            m_psFire.Stop();
        }

    }

    void Fire()
    {
        Projectile newProjectile = Instantiate(projectile).GetComponent<Projectile>();
        transform.LookAt(target);
        newProjectile.transform.rotation = transform.rotation;
        newProjectile.transform.position = transform.position + transform.forward * 0.5f;
        newProjectile.Fire();
        m_psFire.Stop();
        m_psIce.Stop();
    }
}
