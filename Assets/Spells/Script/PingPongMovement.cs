using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PingPongMovement : MonoBehaviour {


	public float movX = 0;
	public float movY = 0;
	public float movZ = 0;


	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		GameObject Spheri = GameObject.Find("Spheri");
		Vector3 pos = Spheri.transform.position;

		pos.x = Mathf.PingPong(Time.time, movX);
		//pos.y = Mathf.PingPong(Time.time, movY);
		//pos.z = Mathf.PingPong(Time.time, movZ);
		Spheri.transform.position = pos;


		
	}
}
