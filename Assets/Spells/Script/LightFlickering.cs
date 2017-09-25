using UnityEngine;
using System.Collections;

public class LightFlickering : MonoBehaviour
{
	private Light lt;
	public Color lightColor = Color.red;

	public float minIntensity = 1.5f;
	public float maxIntensity = 1.6f;
	public float IntSpeed = 1;

	public float minRange = 4f;
	public float maxRange = 4.4f;
	public float RangeSpeed = 0.4f;

    //public float speed = 0.5f;

	void Start()
    {
		lt = GetComponent<Light>();
	}
	void FixedUpdate()
    {
		lt.intensity = minIntensity + Mathf.PingPong(Time.time * IntSpeed /*Mathf.Round(Random.Range(minRange, maxRange))*/, (maxIntensity-minIntensity));
		lt.range =  minRange + Mathf.PingPong(Time.time * RangeSpeed, (maxRange-minRange));
		lt.color = lightColor;
	}
}