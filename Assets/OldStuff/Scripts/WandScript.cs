using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WandScript : MonoBehaviour
{
    private int m_iDeviceIndexThis = -1;
    MagicType m_magictypeLoaded = MagicType.None;
    
    bool m_bIsMagicLoaded = false;

    GameObject m_goSpell;
    public GameObject[] m_goSpellsIce;
    public GameObject[] m_goSpellsFire;
    public GameObject[] m_goSpellsLight;
    //float m_fShotPower = 15f;

    public ParticleSystem m_psIdleFire;
    public ParticleSystem m_psIdleIce;

    public static WandScript m_instanceThis;

    public LineRenderer m_linerendererRange;
    public Text textPanel;
    public Text debugText2;

    public ParticleSystem m_psFail;
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

        Vector2 touchaxis = device.GetAxis();
        if(device.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad))
        {
            debugText2.text = "pressed";
            if (touchaxis.x < 0.0f)
            {
                ChooseSpellform(0);
            }
            else if (touchaxis.x > 0.0f)
            {
                ChooseSpellform(1);
            }

        }
        


        if (m_bIsMagicLoaded && device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
        {
            Projectile newProjectile = Instantiate(m_goSpell).GetComponent<Projectile>();
            newProjectile.transform.rotation = transform.rotation;
            newProjectile.transform.position = transform.GetChild(1).position;
            newProjectile.SetWand(transform);
            newProjectile.Fire();
        }
        if(m_goSpell != null)
        {
            float fSpellrange = m_goSpell.GetComponent<Projectile>().GetRange();
            if (fSpellrange != -1)
            {
                m_linerendererRange.SetPosition(0, transform.position);
                m_linerendererRange.SetPosition(1, transform.position + (transform.forward * fSpellrange));
            }

        }
	}

    public void ChargeMagicType(MagicType _type)
    {
        m_magictypeLoaded = _type;
        if (m_magictypeLoaded == MagicType.Fire)
        {
            m_psIdleFire.Play();
            m_psIdleIce.Stop();
            //projectile = projectileFire;
        }
        if (m_magictypeLoaded == MagicType.Ice)
        {
            m_psIdleIce.Play();
            m_psIdleFire.Stop();
            //projectile = projectileIce;
        }
        if(m_magictypeLoaded == MagicType.Lightning)
        {
            //TODO: Lightningstuff
        }

    }

    public void ChooseSpellform(int _iSpellform)
    {
        switch(m_magictypeLoaded)
        {
            case MagicType.Fire:
                if (m_goSpellsFire.Length >= _iSpellform)
                {
                    m_goSpell = m_goSpellsFire[_iSpellform];
                }
                CheckMagicLoaded();
                break;
            case MagicType.Ice:
                if(m_goSpellsIce.Length >= _iSpellform)
                {
                    m_goSpell = m_goSpellsIce[_iSpellform];
                }
                CheckMagicLoaded();
                break;
            case MagicType.Lightning:
                m_goSpell = m_goSpellsLight[_iSpellform];
                CheckMagicLoaded();
                break;
            default:
                print("There is no correct MagicType loaded");
                break;
        }
    }

    private void CheckMagicLoaded()
    {
        if(m_magictypeLoaded != MagicType.None && m_goSpell != null)
        {
            debugText2.text = m_magictypeLoaded.ToString() + "//" + m_goSpell.name;
            m_bIsMagicLoaded = true;
        }
        else
        {
            m_bIsMagicLoaded = false;
            m_psFail.Play();
        }
    }

    private void ResetWand()
    {
        m_magictypeLoaded = MagicType.None;
        m_bIsMagicLoaded = false;
        m_psIdleFire.Stop();
        m_psIdleIce.Stop();
    }
}
