using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MP_VR_PlayerRegistry : NetworkBehaviour
{
    public static MP_VR_PlayerRegistry s_instance;

    [SyncVar]
    public GameObject m_goPlayer1;
    [SyncVar]
    public GameObject m_goPlayer2;
	// Use this for initialization
	void Start ()
    {
        s_instance = this;
    }

    public void AddPlayer(MP_VR_PlayerController _newPlayer)
    {
        if (m_goPlayer1 == null)
            m_goPlayer1 = _newPlayer.gameObject;
        else if (m_goPlayer2 == null)
            m_goPlayer2 = _newPlayer.gameObject;
        else
            Debug.LogWarning("Too many players");

    }

    public MP_VR_PlayerController FindOpponent(GameObject _playerSelf)
    {
        if (m_goPlayer1 == _playerSelf && m_goPlayer2 != null)
            return m_goPlayer2.GetComponent<MP_VR_PlayerController>();
        else if (m_goPlayer2 == _playerSelf && m_goPlayer1 != null)
            return m_goPlayer1.GetComponent<MP_VR_PlayerController>();
        else
            return null;
    }

}
