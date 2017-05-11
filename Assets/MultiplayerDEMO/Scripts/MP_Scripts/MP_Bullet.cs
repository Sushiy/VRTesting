using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MP_Bullet : MonoBehaviour
{
    public int m_iDamage = 10;
    private Rigidbody m_rigidThis;

    private void Start()
    {
        m_rigidThis = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject hit = collision.gameObject;
        MP_Health health = hit.GetComponent<MP_Health>();
        if (health != null)
        {
            health.TakeDamage(m_iDamage);
            Destroy(gameObject);
        }

    }

    private void Update()
    {
        transform.LookAt(transform.position + m_rigidThis.velocity * 50);
    }

}
