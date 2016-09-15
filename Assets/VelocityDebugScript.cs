using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class VelocityDebugScript : MonoBehaviour
{
    public SteamVR_TrackedObject trackedObj;
    public Slider timerSlider;
    private bool timerOn = false;

    public Slider velx;
    public Slider vely;
    public Slider velz;
	// Use this for initialization
	void Start ()
    {
	
	}

    void Update()
    {
        if(timerSlider != null && velx != null && vely != null && velz != null)
        {
            var device = SteamVR_Controller.Input((int)trackedObj.index);
            Vector3 v3velocity = device.velocity;
            Vector3 v3angular = device.angularVelocity;
            if (device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
            {
                timerOn = true;
            }
            else if (device.GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
            {
                timerOn = false;
                timerSlider.value = 0;
            }

            if (timerOn)
            {
                timerSlider.value += Time.deltaTime;
                velx.value = v3velocity.x;
                vely.value = v3velocity.y;
                velz.value = v3velocity.z;
            }
        }
        
    }
}
