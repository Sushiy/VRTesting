using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MP_VR_NetworkHMD : MonoBehaviour
{

    private MP_VR_PlayerController m_vrplayerctrlThis;
    private Transform m_transVRHMD;

    private void Start()
    {
        m_vrplayerctrlThis = GetComponentInParent<MP_VR_PlayerController>();
        m_transVRHMD = m_vrplayerctrlThis.m_vrplayerThis.hmdTransform;
    }

    private void Update()
    {
        if (m_transVRHMD != null)
        {
            transform.position = m_transVRHMD.position;
            transform.rotation = m_transVRHMD.rotation;
        }
        else
        {
            m_transVRHMD = m_vrplayerctrlThis.m_vrplayerThis.hmdTransform;
        }
    }
}
