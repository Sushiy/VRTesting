using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NetworkMatchLister : MonoBehaviour
{
    public GameObject buttonPrefab;

    void Start()
    {
    }

    public void ListMatches()
    {
        foreach(var match in NetworkManager.singleton.matches)
        {
            GameObject go = GameObject.Instantiate(buttonPrefab, transform);
            go.GetComponentInChildren<Text>().text = "Join: " + match.name;
            go.GetComponentInChildren<MatchJoiner>().matchNumber = NetworkManager.singleton.matches.IndexOf(match);
        }                
    }
}
