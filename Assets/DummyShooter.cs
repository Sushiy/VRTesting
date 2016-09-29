using UnityEngine;
using System.Collections;

public class DummyShooter : MonoBehaviour
{
    public GameObject projectile;

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
            Fire();
        }
    }

    void Fire()
    {
        Projectiles newProjectile = Instantiate(projectile).GetComponent<Projectiles>();
        transform.LookAt(target);
        newProjectile.transform.rotation = transform.rotation;
        newProjectile.transform.position = transform.position + transform.forward * 0.5f;
        newProjectile.Fire();
    }
}
