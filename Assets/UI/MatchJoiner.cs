using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class MatchJoiner : MonoBehaviour
{
    public int matchNumber = 0;

    public void JoinMatch()
    {
        NetworkHUD.s_instance.JoinMatch(matchNumber);
    }
}
