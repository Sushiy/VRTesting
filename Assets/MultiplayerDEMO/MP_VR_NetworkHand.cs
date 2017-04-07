using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MP_VR_NetworkHand : MonoBehaviour
{
    public bool isLeft = false;
    private MP_VR_PlayerController m_vrplayerctrlThis;
    private Transform m_transVRHand;

    private void Start()
    {
        m_vrplayerctrlThis = GetComponentInParent<MP_VR_PlayerController>();
        if (isLeft)
            m_transVRHand = m_vrplayerctrlThis.m_handLeft.transform;
        else
            m_transVRHand = m_vrplayerctrlThis.m_handRight.transform;
    }

    private void Update()
    {
        if (m_transVRHand != null)
        {
            transform.position = m_transVRHand.position;
            transform.rotation = m_transVRHand.rotation;
        }
        else if(m_vrplayerctrlThis != null)
        {
            if (isLeft)
                m_transVRHand = m_vrplayerctrlThis.m_handLeft.transform;

            else
                m_transVRHand = m_vrplayerctrlThis.m_handRight.transform;
        }
    }
}
