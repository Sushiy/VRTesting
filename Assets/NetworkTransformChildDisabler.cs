using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkTransformChildDisabler : MonoBehaviour {

    NetworkTransformChild[] children;
	// Use this for initialization
	void Start ()
    {
        children = GetComponents<NetworkTransformChild>();
	}
	
	// Update is called once per frame
	IEnumerator CheckChildren ()
    {
        while(true)
        {
            yield return new WaitForSeconds(1.0f);
            foreach (NetworkTransformChild child in children)
            {
                if (child.target == null)
                {
                    child.enabled = false;
                }
            }
        }
	}
}
