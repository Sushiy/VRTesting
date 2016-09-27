using UnityEngine;
using UnityEngine.UI;
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

    public static WandScript m_instanceThis;

    public LineRenderer m_linerendererRange;
    public Text textPanel;
    // Use this for initialization
    void Awake ()
    {
        m_instanceThis = GetComponent<WandScript>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (m_iDeviceIndexThis == -1)
            m_iDeviceIndexThis = (int)GetComponent<SteamVR_TrackedObject>().index;
        var device = SteamVR_Controller.Input(m_iDeviceIndexThis);

        if (isMagicLoaded && (device.GetPressDown(SteamVR_Controller.ButtonMask.Grip))|| device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
        {
            Projectiles newProjectile = Instantiate(projectile).GetComponent<Projectiles>();
            newProjectile.transform.rotation = transform.rotation;
            newProjectile.transform.position = transform.GetChild(1).position;
            newProjectile.SetWand(transform);
            newProjectile.Fire();
            isMagicLoaded = false;
            fire.Stop();
            ice.Stop();
        }

        m_linerendererRange.SetPosition(0, transform.position);
        m_linerendererRange.SetPosition(1, transform.position + (transform.forward * WireMissileScript.m_fMaxRange));
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
