using UnityEngine;
using System.Collections;

public enum MagicType
{
    None = 0,
    Fire,
    Water,
    Lightning
};

public class MagicTypeChooser : MonoBehaviour
{

    /*
     * This Script is controlled with the offhand.
     * It controls the color/type of magic you will be casting. The Type will be shown in the form of a magicsphere that floats on top of the controller.
     * For now this will be done by clicking the correspondingly colored Button on the trackpad. Controller gestures could later replace this.
     * 
     * This Script also transfers the magictype to the mainhand, as soon as the mainhand controller enters the magic sphere.
     */

    public GameObject m_goMainHandController; //The gameobject of the mainhandcontroller or in this case the tip of the wand controlled with the mainhand.

    private int m_iDeviceIndexThis = -1; //The deviceindex of the offhandcontroller. Is calculated at the beginning of the game.

    MagicType activeMagic = MagicType.None;
    
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
        Vector2 touchaxis = device.GetAxis();
        if (touchaxis.x < 0)
        {
            transform.GetChild(1).GetComponent<MeshRenderer>().material.color = new Color(0, 0, 1, 0.5f);
            activeMagic = MagicType.Water;
        }
        else if(touchaxis.x > 0)
        {
            transform.GetChild(1).GetComponent<MeshRenderer>().material.color = new Color(1, 0, 0, 0.5f);
            activeMagic = MagicType.Fire;
        }
            
	}

    public void OnTriggerEnter(Collider _coll)
    {
        if(_coll.CompareTag("Wand"))
        {
            //Debug.Log("Wand has entered the magic sphere. Now transferring power");
            m_goMainHandController.GetComponent<WandScript>().LoadMagic(activeMagic);
        }
    }
}
