using UnityEngine;
using System.Collections;

public class WandScript : MonoBehaviour
{
    private int m_iDeviceIndexThis = -1;
    MagicType loadedType = MagicType.None;
    
    bool isMagicLoaded = false;

    GameObject projectile;
    public GameObject projectileIce;
    public GameObject projectileFire;
    float ShotPower = 15f;

    public ParticleSystem fire;
    public ParticleSystem ice;
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

        if (isMagicLoaded && (device.GetPressDown(SteamVR_Controller.ButtonMask.Grip))|| device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
        {
            GameObject newProjectile = Instantiate(projectile);
            newProjectile.transform.rotation = transform.rotation;
            newProjectile.transform.position = transform.GetChild(1).position;
            Rigidbody rbProjectile = newProjectile.GetComponent<Rigidbody>();
            rbProjectile.AddForce(transform.forward * 15f, ForceMode.Impulse);
            isMagicLoaded = false;
            fire.Stop();
            ice.Stop();
        }
	}

    public void LoadMagic(MagicType _type)
    {
        loadedType = _type;
        isMagicLoaded = true;
        if (loadedType == MagicType.Fire)
        {
            fire.Play();
            ice.Stop();
            projectile = projectileFire;
        }
        if (loadedType == MagicType.Water)
        {
            ice.Play();
            fire.Stop();
            projectile = projectileIce;
        }

    }
}
