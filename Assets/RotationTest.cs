using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Quaternion rotation = Quaternion.FromToRotation(transform.forward, Vector3.forward);
        transform.rotation = rotation;
    }

}
