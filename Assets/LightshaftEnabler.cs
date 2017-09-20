using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightshaftEnabler : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        Camera camera = GetComponentInChildren<Camera>();
        GameObject[] lightshaftObjects = GameObject.FindGameObjectsWithTag("lightshafts");
        foreach(GameObject g in lightshaftObjects)
        {
            g.GetComponent<LightShafts>().m_Cameras[0] = camera;
        }
	}
}
