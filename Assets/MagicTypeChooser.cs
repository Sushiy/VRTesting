using UnityEngine;
using System.Collections;

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

    
	// Use this for initialization
	void Start ()
    {
	    
	}
	
	// Update is called once per frame
	void Update ()
    {
	    
	}

    public void OnTriggerEnter(Collider _coll)
    {
        if(_coll.CompareTag(m_goMainHandController.tag))
        {
            Debug.Log("Wand has entered the magic sphere. Now transferring power");

        }
    }
}
