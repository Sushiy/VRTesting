using UnityEngine;
using System.Collections;

public class EmitterScript : MonoBehaviour
{

    public SteamVR_Controller.DeviceRelation deviceRelation = SteamVR_Controller.DeviceRelation.Rightmost;
    public ParticleSystem particles;
    private int deviceIndex = -1;
	// Use this for initialization
	void Start ()
    {
        deviceIndex = SteamVR_Controller.GetDeviceIndex(deviceRelation);
    }
	
	// Update is called once per frame
	void Update ()
    {
        if(deviceIndex == -1)
            deviceIndex = SteamVR_Controller.GetDeviceIndex(deviceRelation);

        if (deviceIndex != -1)
        {

            if (SteamVR_Controller.Input(deviceIndex).GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
            {
                SteamVR_Controller.Input(deviceIndex).TriggerHapticPulse(1000);
                particles.Play();
            }
            else if (SteamVR_Controller.Input(deviceIndex).GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
            {
                particles.Stop();
            }

            if (SteamVR_Controller.Input(deviceIndex).GetPressDown(SteamVR_Controller.ButtonMask.Grip))
            {
                particles.Clear();
            }

        }
        
	}
}
