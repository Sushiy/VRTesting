using UnityEngine;
using System.Collections;

public class LightFlickering : MonoBehaviour
{
	private Light lt;
	public Color lightColor = Color.red;

	public float minIntensity = 1.6f;
	public float Intensity = 1.5f;
	public float intSpeed = 1f;


	public float minRange = 4f;
    public float Range = 0.4f;
	public float rangSpeed = 1f;



	void Start()
    {
		lt = GetComponent<Light>();
	}
	void FixedUpdate()
    {
		lt.intensity = minIntensity + Mathf.PingPong(Time.time * intSpeed, Intensity);
		lt.range =  minRange + Mathf.PingPong(Time.time * rangSpeed, Range);
		lt.color = lightColor;
	}
}