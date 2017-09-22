using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//This class syncs the characters Head with the HeadMountedDisplays actual positions
public class MP_VR_NetworkHMD : MonoBehaviour
{

    private IPlayerController m_vrplayerctrlThis;
    private Transform m_transVRHMD;

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        m_vrplayerctrlThis = GetComponentInParent<IPlayerController>();
        if (m_vrplayerctrlThis != null)
            m_transVRHMD = m_vrplayerctrlThis.GetVRPlayer().hmdTransform;
    }

    private void Update()
    {
        if (m_vrplayerctrlThis == null)
        {
            Init();
            return;
        }

        if (m_transVRHMD != null)
        {
            transform.position = m_transVRHMD.position;
            transform.rotation = m_transVRHMD.rotation;
        }
        else
        {
            if(m_vrplayerctrlThis.GetVRPlayer()!= null)
                m_transVRHMD = m_vrplayerctrlThis.GetVRPlayer().hmdTransform;
        }
    }
}
