using UnityEngine;
using System.Collections;

public class LightFlickering : MonoBehaviour
{
	private Light lt;
	public Color lightColor = Color.red;

	public float minIntensity = 1.5f;
	public float maxIntensity = 1.6f;

	public float minRange = 4f;
    public float Range = 0.4f;
	public float maxRange = 4.4f;

	void Start()
    {
		lt = GetComponent<Light>();
	}
	void FixedUpdate()
    {
		lt.intensity = minIntensity + Mathf.PingPong(Time.time * Mathf.Round(Random.Range(minRange, maxRange)), maxIntensity);
		lt.range =  minRange + Mathf.PingPong(Time.time * Mathf.PingPong(Time.time, maxIntensity), Range);
		lt.color = lightColor;
	}
}