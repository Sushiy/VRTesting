using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkHUD : MonoBehaviour {

    NetworkManager manager;
	// Use this for initialization
	void Start ()
    {
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
}
