using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MP_VR_NetworkManagerExtension : NetworkManager
{ 
    public GameObject m_goPlayer1;

    public GameObject m_goPlayer2;

     public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        base.OnServerAddPlayer(conn, playerControllerId);
        if (conn.playerControllers.Count > 0)
        {
            GameObject player = conn.playerControllers[0].gameObject;
            if(m_goPlayer1 == null)
            {
                m_goPlayer1 = player;
            }
            else if (m_goPlayer2 == null)
            {
                m_goPlayer2 = player;
            }
            else
                Debug.LogWarning("Too many players");
        }
        if(conn.playerControllers.Count > 1)
        {
            SetOpponents();
        }
    }

    void SetOpponents()
    {
        if (m_goPlayer1 != null && m_goPlayer2 != null)
        {
            m_goPlayer1.GetComponent<MP_VR_PlayerController>().SetOpponent(m_goPlayer2.GetComponent<MP_VR_PlayerController>());
            m_goPlayer2.GetComponent<MP_VR_PlayerController>().SetOpponent(m_goPlayer1.GetComponent<MP_VR_PlayerController>());

        }
    }

    public void OnPlayer1(GameObject g)
    {
        Debug.Log("player1 changed");
    }

    public void OnPlayer2(GameObject g)
    {
        Debug.Log("player2 changed");
    }
}
