using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

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
        manager.matchMaker.ListMatches(0, 20, "", true, 0, 0, OnInternetMatchList);
    }

    private void OnInternetMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matches)
    {
        if (success)
        {
            if (matches.Count != 0)
            {
                //Debug.Log("A list of matches was returned");

                //join the last server (just in case there are two...)
                NetworkManager.singleton.matchMaker.JoinMatch(matches[matches.Count - 1].networkId, "", "", "", 0, 0, OnJoinInternetMatch);
            }
            else
            {
                Debug.Log("No matches in requested room!");
            }
        }
        else
        {
            Debug.LogError("Couldn't connect to match maker");
        }
    }

    //this method is called when your request to join a match is returned
    private void OnJoinInternetMatch(bool success, string extendedInfo, MatchInfo matchInfo)
    {
        if (success)
        {
            //Debug.Log("Able to join a match");

            MatchInfo hostInfo = matchInfo;
            NetworkManager.singleton.StartClient(hostInfo);
        }
        else
        {
            Debug.LogError("Join match failed");
        }
    }

    public void JoinMatch(UnityEngine.Networking.Match.MatchInfoSnapshot match)
    {
        SetMatchName(match.name);
        manager.matchSize = (uint)match.currentSize;
        manager.matchMaker.JoinMatch(match.networkId, "","","",0,0, manager.OnMatchJoined);
    }

    public void JoinMatch(int matchIndex)
	{
		if (manager.matches != null) 
		{
			UnityEngine.Networking.Match.MatchInfoSnapshot match = manager.matches [matchIndex];
			SetMatchName (match.name);
			manager.matchSize = (uint)match.currentSize;
			manager.matchMaker.JoinMatch (match.networkId, "", "", "", 0, 0, manager.OnMatchJoined);
		}
		else
			Debug.LogError ("There is no active Match to join.");
    }
}
