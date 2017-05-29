using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CirclingObject : MonoBehaviour
{
    public float radius = 1.0f;
    public float speed = 6.0f;
    Vector3 Startposition;
	// Use this for initialization
	void Start () {
        Startposition = transform.position;
    }
	
	// Update is called once per frame
	void Update () {

        Vector3 oldPosition = transform.position;
        float speedScale = (2 * (float)Mathf.PI) / speed;
        float angle = Time.time * speedScale;
        float x = Startposition.x + Mathf.Sin(angle) * radius;
        float z = Startposition.z + Mathf.Cos(angle) * radius;
        transform.position = new Vector3(x, Startposition.y, z);
        transform.LookAt(transform.position + (transform.position - oldPosition) * 10f);

    }
}
