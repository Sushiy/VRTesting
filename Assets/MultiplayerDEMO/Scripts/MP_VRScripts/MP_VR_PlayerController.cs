using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Valve.VR;

public class MP_VR_PlayerController : NetworkBehaviour
{
    public GameObject m_prefabVRStation;

    public Valve.VR.InteractionSystem.Player m_vrplayerThis;

    public GameObject m_prefabBullet;

    public Valve.VR.InteractionSystem.Hand m_handRight;
    public Valve.VR.InteractionSystem.Hand m_handLeft;
    public MP_VR_NetworkHand m_mpvrhand1;
    public MP_VR_NetworkHand m_mpvrhand2;
    public Transform hand1Spawn;
    public Transform hand2Spawn;

    private ForceRecorder m_forcerecThis;
    private MagicWand m_magicwandThis;
	
	// Update is called once per frame
	void Update ()
    {
        //Check if you are a local Player
        if (!isLocalPlayer)
        {
            return;
        }
        //Check if you have found your ForceRecorder
        if(m_forcerecThis == null)
        { 
            InitSpellComponents();
            return;
        }
        //Check if you have found your hands
        if (m_handLeft == null || m_handRight == null)
        {
            CheckHands();
            return;
        }

        if (m_handRight != null && m_handRight.controller != null && m_handRight.controller.GetHairTriggerDown())
        {
            CmdFire(hand1Spawn.transform.position, hand1Spawn.transform.rotation);
        }
        if (m_handLeft != null && m_handLeft.controller != null && m_handLeft.controller.GetHairTriggerDown())
        {
            CmdFire(hand2Spawn.transform.position, hand2Spawn.transform.rotation);
        }
        if(m_forcerecThis.isFiring())
        {
            CmdFireSpell(m_forcerecThis.m_v3velocity);
        }
    }

    [Command] //Command is called on client and executed on the server
    void CmdFire(Vector3 p, Quaternion q)
    {
        // Create the Bullet from the Bullet Prefab
        GameObject bullet = (GameObject)Instantiate(
            m_prefabBullet,
            p,
            q);

        // Add velocity to the bullet
        bullet.GetComponent<Rigidbody>().velocity = -bullet.transform.up * 12;

        // Spawn the bullet on the Clients
        NetworkServer.Spawn(bullet);

        // Destroy the bullet after 2 seconds
        Destroy(bullet, 2.0f);
    }

    [Command] //Command is called on client and executed on the server
    void CmdFireSpell(Vector3 velocity)
    {
        GameObject spell;
        if (m_magicwandThis.LoadedSpell == SpellType.NONE) return;

        if (m_magicwandThis.LoadedSpell == SpellType.FIREBALL)
        {
            spell = Instantiate<GameObject>(m_magicwandThis.prefab_Fireball);
            spell.transform.position = m_magicwandThis.m_SpawnPoint.position;
            spell.GetComponent<Rigidbody>().velocity = (velocity * 3.0f);

            // Spawn the bullet on the Clients
            NetworkServer.Spawn(spell);
            // Destroy the bullet after 2 seconds
            Destroy(spell, 5.0f);
        }

        m_magicwandThis.LoadWand(SpellType.NONE);


    }

    public override void OnStartLocalPlayer()
    {
        m_vrplayerThis = GameObject.Instantiate(m_prefabVRStation,transform.position, transform.rotation).GetComponent< Valve.VR.InteractionSystem.Player>();
        m_handRight = m_vrplayerThis.rightHand;
        m_handLeft = m_vrplayerThis.leftHand;
        CheckHands();
        InitSpellComponents();
    }

    private void InitSpellComponents()
    {
        if (m_forcerecThis != null && m_magicwandThis != null)
            return;

        m_forcerecThis = m_vrplayerThis.GetComponentInChildren<ForceRecorder>();
        if (m_forcerecThis != null)
            m_forcerecThis.RemoveFromParent();
        m_magicwandThis = m_vrplayerThis.GetComponentInChildren<MagicWand>();
    } 

    private void CheckHands()
    {
        if (m_handRight != null && m_handLeft != null && m_handLeft != m_handRight)
            return;

        if(m_vrplayerThis.hands.Length > 1)
        {
            m_handRight = m_vrplayerThis.rightHand;
            m_handLeft = m_vrplayerThis.leftHand;
            m_mpvrhand1.m_transVRHand = m_handRight.transform;
            m_mpvrhand2.m_transVRHand = m_handLeft.transform;

        }
        else
        {
            Debug.Log("No hands were found");
        }
    }
}
