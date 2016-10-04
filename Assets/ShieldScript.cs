using UnityEngine;
using System.Collections;

public class ShieldScript : MonoBehaviour
{
    private int m_iDeviceIndexThis = -1;
    public GameObject m_goShield;
    public Collider m_collShield;

    public float m_fShieldRecharge = 2.0f;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (m_iDeviceIndexThis == -1)
            m_iDeviceIndexThis = (int)GetComponent<SteamVR_TrackedObject>().index;
        var device = SteamVR_Controller.Input(m_iDeviceIndexThis);

        if (device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
        {
            switch (MagicTypeChooser.s_Instance.activeMagic)
            {
                case MagicType.Fire:
                    m_goShield.GetComponent<MeshRenderer>().material.color = new Color(1, 0, 0, 0.6f);
                    break;
                case MagicType.Ice:
                    m_goShield.GetComponent<MeshRenderer>().material.color = new Color(0, 0, 1, 0.6f);
                    break;
                default:
                    break;
            }
            m_goShield.SetActive(true);
        }
        else if (device.GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
            m_goShield.SetActive(false);
    }

    public void OnTriggerEnter(Collider _coll)
    {
        print("ShieldTriggerEntered");
        if (_coll.CompareTag("Projectile"))
        {
            print("Hit by Projectile");
            MagicType magictypeProjectile = _coll.GetComponent<Projectile>().m_magicTypeProjectile;
            int iMagicResult = MagicTypeChooser.s_Instance.DecideMagicWinner(magictypeProjectile);
            if (iMagicResult != 2)
            {
                print("ShieldBreak!");
                m_collShield.enabled = false;
                m_goShield.GetComponent<MeshRenderer>().enabled = false;
                StartCoroutine(ReactivateShield());
            }
        }
    }

    IEnumerator ReactivateShield()
    {
        yield return new WaitForSeconds(2.0f);
        m_collShield.enabled = true;
    }
}