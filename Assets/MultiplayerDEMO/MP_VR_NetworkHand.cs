using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MP_VR_NetworkHand : MonoBehaviour
{
    public Transform m_transVRHand;

    private void Start()
    {
    }

    private void Update()
    {
        if (m_transVRHand != null)
        {
            transform.position = m_transVRHand.position;
            transform.rotation = m_transVRHand.rotation;
        }
    }
}
