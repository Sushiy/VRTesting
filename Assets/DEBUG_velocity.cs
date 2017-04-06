using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class DEBUG_velocity : MonoBehaviour {

    VelocityEstimator vEstimator;
	// Use this for initialization
	void Start ()
    {
        vEstimator = GetComponent<VelocityEstimator>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        Gizmos.DrawRay(transform.position, vEstimator.GetVelocityEstimate()*10.0f);
	}
}
