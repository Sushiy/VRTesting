using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MP_VR_ViveTransformSync : NetworkBehaviour
{
    [SyncVar]
    private Vector3 m_v3Hand1Pos;
    [SyncVar]
    private Vector3 m_v3Hand2Pos;
    [SyncVar]
    private Quaternion m_qHand1Rot;
    [SyncVar]
    private Quaternion m_qHand2Rot;

    [SyncVar]
    private Vector3 m_v3HeadPos;
    [SyncVar]
    private Quaternion m_qHeadRot;

    public Transform m_transHand1;
    public Transform m_transHand2;
    public Transform m_transHead;

    private float m_fUpdateInterval;

    // Update is called once per frame
    void Update()
    {
        if (isLocalPlayer)
        {
            // update the server with position/rotation
            m_fUpdateInterval += Time.deltaTime;
            if (m_fUpdateInterval > 0.11f) // 9 times per second
            {
                m_fUpdateInterval = 0;
                CmdSync(m_transHand1.position, m_transHand1.rotation, m_transHand2.position, m_transHand2.rotation, m_transHead.position, m_transHead.rotation);
            }
        }
        else
        {
            m_transHand1.position = Vector3.Lerp(m_transHand1.position, m_v3Hand1Pos, 0.1f);
            m_transHand1.rotation = Quaternion.Lerp(m_transHand1.rotation, m_qHand1Rot, 0.1f);
            m_transHand2.position = Vector3.Lerp(m_transHand2.position, m_v3Hand2Pos, 0.1f);
            m_transHand2.rotation = Quaternion.Lerp(m_transHand2.rotation, m_qHand2Rot, 0.1f);
            m_transHead.position = Vector3.Lerp(m_transHead.position, m_v3HeadPos, 0.1f);
            m_transHead.rotation = Quaternion.Lerp(m_transHead.rotation, m_qHeadRot, 0.1f);

        }
    }

    void CmdSync(Vector3 _v3Hand1, Quaternion _qHand1, Vector3 _v3Hand2, Quaternion _qHand2, Vector3 _v3Head, Quaternion _qHead)
    {
        m_v3Hand1Pos = m_transHand1.position;
        m_qHand1Rot = m_transHand1.rotation;
        m_v3Hand2Pos = m_transHand2.position;
        m_qHand2Rot = m_transHand2.rotation;
        m_v3HeadPos = m_transHead.position;
        m_qHeadRot = m_transHead.rotation;
    }
}