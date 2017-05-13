using UnityEngine;
using System.Collections;

public enum MagicType
{
    None = 0,
    Fire,
    Ice,
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

    public MagicType activeMagic = MagicType.None;

    public ParticleSystem m_psFire;
    public ParticleSystem m_psIce;

    public static MagicTypeChooser s_Instance;
	// Use this for initialization
	void Start ()
    {
        s_Instance = this;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (m_iDeviceIndexThis == -1)
            m_iDeviceIndexThis = (int)GetComponent<SteamVR_TrackedObject>().index;

        var device = SteamVR_Controller.Input(m_iDeviceIndexThis);
        Vector2 touchaxis = device.GetAxis();
        if (touchaxis.x < -0.2f)
        {
            transform.GetChild(1).GetComponent<MeshRenderer>().material.color = new Color(0, 0, 1, 0.5f);
            activeMagic = MagicType.Ice;
            m_psIce.Play();
            m_psFire.Stop();
        }
        else if(touchaxis.x > 0.2f)
        {
            transform.GetChild(1).GetComponent<MeshRenderer>().material.color = new Color(1, 0, 0, 0.5f);
            activeMagic = MagicType.Fire;
            m_psFire.Play();
            m_psIce.Stop();
        }
            
	}

    public void OnTriggerEnter(Collider _coll)
    {
        if(_coll.CompareTag("Wand"))
        {
            Debug.Log("Wand has entered the magic sphere. Now transferring power");
            m_goMainHandController.GetComponent<WandScript>().ChargeMagicType(activeMagic);
        }
    }

    public int DecideMagicWinner(MagicType _magicProjectile)
    {
        //This compares the three magictypes
        //If the shield wins, this returns 1
        //If the projectile wins, this returns 0
        //if there is a draw, this returns 2
        //if there is no result this returns -1
        if (_magicProjectile == activeMagic)
        {
            return 2;
        }
        else if ((activeMagic == MagicType.Fire && _magicProjectile == MagicType.Ice) ||
            (activeMagic == MagicType.Ice && _magicProjectile == MagicType.Lightning) ||
            (activeMagic == MagicType.Lightning && _magicProjectile == MagicType.Fire))
        {
            return 1;
        }
        else if ((activeMagic == MagicType.Fire && _magicProjectile == MagicType.Lightning) ||
            (activeMagic == MagicType.Ice && _magicProjectile == MagicType.Fire) ||
            (activeMagic == MagicType.Lightning && _magicProjectile == MagicType.Ice))
        {
            return 0;
        }

        return -1;
    }
}
