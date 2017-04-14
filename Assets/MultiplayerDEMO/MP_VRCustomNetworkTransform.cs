using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MP_VRCustomNetworkTransform: NetworkBehaviour
{
    [SyncVar]
    private Vector3 m_v3TransformPos;
    [SyncVar]
    private Quaternion m_qTransformRot;

    public int m_iUpdatesPerSecond = 9;
    private float m_fUpdateInterval;
    private float m_fUpdateDelta;

    private void Awake()
    {
        m_fUpdateInterval = 1.0f / m_iUpdatesPerSecond;
    }
    // Update is called once per frame
    void Update()
    {
        if (isLocalPlayer)
        {
            // update the server with position/rotation
            m_fUpdateDelta += Time.deltaTime;
            if (m_fUpdateDelta > m_fUpdateInterval)
            {
                m_fUpdateDelta = 0;
                CmdSync(transform.position, transform.rotation);
            }
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, m_v3TransformPos, m_fUpdateInterval);
            transform.rotation = Quaternion.Lerp(transform.rotation, m_qTransformRot, m_fUpdateInterval);

        }
    }

    void CmdSync(Vector3 _v3Object, Quaternion _qObject)
    {
        Debug.Log("CmdSync on Player");
        m_v3TransformPos = transform.position;
        m_qTransformRot = transform.rotation;
    }
}