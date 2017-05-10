﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Valve.VR;

public class MP_VR_PlayerController : NetworkBehaviour
{
    public bool m_bIsReady = false;

    [SerializeField]
    private GameObject m_prefabVRStation;
    [SerializeField]
    private GameObject m_prefabBullet;
    [SerializeField]
    private MP_VR_NetworkHand m_mpvrhand1;
    [SerializeField]
    private MP_VR_NetworkHand m_mpvrhand2;
    [SerializeField]
    private Transform hand1Spawn;
    [SerializeField]
    private Transform hand2Spawn;

    private Valve.VR.InteractionSystem.Player m_vrplayerThis;

    public Valve.VR.InteractionSystem.Player ValvePlayer
    {
        get
        {
            return m_vrplayerThis;
        }
    }

    private Valve.VR.InteractionSystem.Hand m_handRight;
    private Valve.VR.InteractionSystem.Hand m_handLeft;
    private ForceRecorder m_forcerecThis;
    private MagicWand m_magicwandThis;

    //This is used instead of Start for Initialization (only called by local player)
    public override void OnStartLocalPlayer()
    {
        //Spawn the SteamVR player prefab
        m_vrplayerThis = GameObject.Instantiate(m_prefabVRStation, transform.position, transform.rotation).GetComponent<Valve.VR.InteractionSystem.Player>();
        //Grab the SteamVR Hands
        m_handRight = m_vrplayerThis.rightHand;
        m_handLeft = m_vrplayerThis.leftHand;
        //Check the hands again
        CheckHands();
        //Grab the forcerecorder and wand
        InitSpellComponents();
    }

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
            m_bIsReady = false;
            InitSpellComponents();
            return;
        }
        //Check if you have found your hands
        if (m_handLeft == null || m_handRight == null)
        {
            m_bIsReady = false;
            CheckHands();
            return;
        }
        //if you weren't ready yet and made it this far, get ready
        if(!m_bIsReady)
            m_bIsReady = true;

        //If the forcerecorder wants us to fire spells, do it
        if(m_forcerecThis.isFiring())
        {
            CmdFireSpell(m_forcerecThis.m_v3velocity);
        }

        /******** This fires debug bullets*****/
        /*
        if (m_handRight != null && m_handRight.controller != null && m_handRight.controller.GetHairTriggerDown())
        {
            CmdFire(hand1Spawn.transform.position, hand1Spawn.transform.rotation);
        }
        if (m_handLeft != null && m_handLeft.controller != null && m_handLeft.controller.GetHairTriggerDown())
        {
            CmdFire(hand2Spawn.transform.position, hand2Spawn.transform.rotation);
        }*/
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
        //if you haven't loaded a Spell, return
        if (m_magicwandThis.LoadedSpell == SpellType.NONE) return;
        
        GameObject goSpell;
        if (m_magicwandThis.LoadedSpell == SpellType.FIREBALL)
        {
            //Instatiate the SpellObject and shoot it 
            goSpell = Instantiate<GameObject>(m_magicwandThis.prefab_Fireball);
            goSpell.transform.position = m_magicwandThis.m_SpawnPoint.position;
            goSpell.GetComponent<Rigidbody>().velocity = (velocity * 3.0f);

            // Spawn the spellObject on the Clients
            NetworkServer.Spawn(goSpell);
            // Destroy the spellObject after 2 seconds
            Destroy(goSpell, 5.0f);
        }

        m_magicwandThis.LoadWand(SpellType.NONE);
    }


    //Find ForceRecorder and MagicWand Components
    private void InitSpellComponents()
    {
        if (m_forcerecThis != null && m_magicwandThis != null)
            return;

        m_forcerecThis = m_vrplayerThis.GetComponentInChildren<ForceRecorder>();
        if (m_forcerecThis != null)
            m_forcerecThis.RemoveFromParent();
        m_magicwandThis = m_vrplayerThis.GetComponentInChildren<MagicWand>();
    } 

    //Check if you currently have all the necessary handreferences
    private void CheckHands()
    {
        if (m_handRight != null && m_handLeft != null && m_handLeft != m_handRight)
            return;

        if(m_vrplayerThis.hands.Length > 1)
        {
            m_handRight = m_vrplayerThis.rightHand;
            m_handLeft = m_vrplayerThis.leftHand;
            if (m_handRight != null && m_handLeft != null)
            {
                m_mpvrhand1.m_transVRHand = m_handRight.transform;
                m_mpvrhand2.m_transVRHand = m_handLeft.transform;
            }
        }
        else
        {
            Debug.Log("No hands were found");
        }
    }
}