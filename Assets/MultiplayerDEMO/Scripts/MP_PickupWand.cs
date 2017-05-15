using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MP_PickupWand : MonoBehaviour
{
    [SerializeField]
    private GameObject m_prefabWand;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hand"))
        {
            Instantiate<GameObject>(m_prefabWand, other.transform);
            Destroy(gameObject);
        }
    }
}
