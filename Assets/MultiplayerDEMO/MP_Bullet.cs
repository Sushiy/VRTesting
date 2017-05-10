using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MP_Bullet : MonoBehaviour
{
    public int m_iDamage = 10;

    private void OnCollisionEnter(Collision collision)
    {
        GameObject hit = collision.gameObject;
        MP_Health health = hit.GetComponent<MP_Health>();
        if (health != null)
        {
            health.TakeDamage(m_iDamage);
        }

        Destroy(gameObject);
    }

}
