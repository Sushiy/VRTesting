using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Valve.VR.InteractionSystem;

public class MP_PickupWand : MonoBehaviour
{
    [SerializeField]
    private GameObject m_prefabWand;
    [SerializeField]
    private GameObject m_prefabOffhand;
    [SerializeField]
    private bool m_bSpawnOffhand = true;
    [SerializeField]
    private bool m_bDestroyCollidersAfterUse = true;

    private void Awake()
    {
        Assert.IsNotNull(m_prefabWand);
        Assert.IsNotNull(m_prefabOffhand);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hand"))
        {
            Transform offhand = other.GetComponent<Hand>().otherHand.transform;

            GameObject mainWand = Instantiate(m_prefabWand, other.transform);
            mainWand.GetComponent<MagicWand>().setMainHand();
            if (m_bSpawnOffhand)
                Instantiate(m_prefabOffhand, offhand);

            if (m_bDestroyCollidersAfterUse)
            {
                Destroy(other);
                Destroy(offhand.GetComponent<Collider>());
            }

            Destroy(gameObject);
        }
    }
}
