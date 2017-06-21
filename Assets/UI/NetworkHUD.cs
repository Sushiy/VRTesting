﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkHUD : MonoBehaviour
{
    public static NetworkHUD s_instance;
    NetworkManager manager;
	// Use this for initialization
	void Start ()
    {
        if (s_instance != null)
        {
            Debug.LogError("Two NetworkHUDs");
            Destroy(this);
        }

        s_instance = this;
        manager = GetComponent<NetworkManager>();	
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void StartServer()
    {
        manager.StartServer();
    }

    public void StartHost()
    {
        manager.StartHost();
    }

    public void StopHost()
    {
        manager.StopHost();
    }

    public void StartClient()
    {
        manager.StartClient();
    }

    /******INTERNET MATCHMAKING******/
    public void StartMatchmaker()
    {
        manager.StartMatchMaker();
    }

    public void StopMatchmaker()
    {
        manager.StopMatchMaker();
    }

    public void SetMatchName(string matchName)
    {
        manager.matchName = matchName;
    }

    public void CreateMatch()
    {
        manager.matchMaker.CreateMatch(manager.matchName, manager.matchSize, true, "", "", "", 0, 0, manager.OnMatchCreate);
    }

    public void ListMatches()
    {
        manager.matchMaker.ListMatches(0, 20, "", true, 0, 0, manager.OnMatchList);
    }

    public void JoinMatch(UnityEngine.Networking.Match.MatchInfoSnapshot match)
    {
        SetMatchName(match.name);
        manager.matchSize = (uint)match.currentSize;
        manager.matchMaker.JoinMatch(match.networkId, "","","",0,0, manager.OnMatchJoined);
    }

    public void JoinMatch(int matchIndex)
    {
        UnityEngine.Networking.Match.MatchInfoSnapshot match = manager.matches[matchIndex];
        SetMatchName(match.name);
        manager.matchSize = (uint)match.currentSize;
        manager.matchMaker.JoinMatch(match.networkId, "", "", "", 0, 0, manager.OnMatchJoined);
    }
}