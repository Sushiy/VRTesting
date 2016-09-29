using UnityEngine;
using System.Collections;

public class ShieldScript : MonoBehaviour
{
    private int m_iDeviceIndexThis = -1;
    public GameObject shield;

    // Use this for initialization
    void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (m_iDeviceIndexThis == -1)
            m_iDeviceIndexThis = (int)GetComponent<SteamVR_TrackedObject>().index;
        var device = SteamVR_Controller.Input(m_iDeviceIndexThis);

        if (device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
        {
            switch(MagicTypeChooser.s_Instance.activeMagic)
            {
                case MagicType.Fire:
                    shield.layer = 11;
                    shield.GetComponent<MeshRenderer>().material.color = new Color(1, 0, 0, 0.6f);
                    shield.SetActive(true);
                    break;
                case MagicType.Ice:
                    shield.layer = 12;
                    shield.GetComponent<MeshRenderer>().material.color = new Color(0, 0, 1, 0.6f);
                    shield.SetActive(true);
                    break;
                default:
                    break;
            }
        }
        else if (device.GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
            shield.SetActive(false);
    }
}
