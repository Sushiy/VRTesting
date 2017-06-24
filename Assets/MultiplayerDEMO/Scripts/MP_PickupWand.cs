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
    private bool m_bSpawnOffhand = false;
    [SerializeField]
    private bool m_bDestroyMainHandColliderAfterUse = true;
    [SerializeField]
    private bool m_bDestroyOffHandColliderAfterUse = false;
    [SerializeField]
    private bool m_bDestroyForcerecorderMesh = true;

    private void Awake()
    {
        Assert.IsNotNull(m_prefabWand);
        Assert.IsNotNull(m_prefabOffhand);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hand"))
        {
            foreach(MP_VR_PlayerController p in MP_VR_PlayerRegistry.s_instance.m_list_mpvr_playerctrl)
            {
                if (p.isLocalPlayer)
                {
                    p.SpawnWand(other, m_bSpawnOffhand, m_bDestroyForcerecorderMesh, m_bDestroyOffHandColliderAfterUse, m_bDestroyMainHandColliderAfterUse);
                    Destroy(gameObject);
                }
            }
        }
    }
}
