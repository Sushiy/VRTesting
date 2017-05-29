using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class DestroyPSwhenDone : MonoBehaviour {

    ParticleSystem ps;
	// Use this for initialization
	void Start ()
    {
        ps = GetComponent<ParticleSystem>();
	}
	
	// Update is called once per frame
	void Update ()
    {
		if(!ps.IsAlive())
        {
            Destroy(gameObject);
        }
	}
}
