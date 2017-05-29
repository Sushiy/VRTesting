using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MP_VR_PlayerRegistry : MonoBehaviour
{
    public static MP_VR_PlayerRegistry s_instance;

    private List<MP_VR_PlayerController> m_list_mpvr_playerctrl;

	// Use this for initialization
	void Start ()
    {
        s_instance = this;
        m_list_mpvr_playerctrl = new List<MP_VR_PlayerController>();
    }

    public void AddPlayer(MP_VR_PlayerController _newPlayer)
    {
        m_list_mpvr_playerctrl.Add(_newPlayer);
        foreach(MP_VR_PlayerController player in m_list_mpvr_playerctrl)
        {
            if(player != _newPlayer)
            {
                player.SetOpponent(_newPlayer);
            }
        }
    }

}
