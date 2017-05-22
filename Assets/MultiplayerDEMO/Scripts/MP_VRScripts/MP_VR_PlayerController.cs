using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Valve.VR;

public class MP_VR_PlayerController : NetworkBehaviour
{
    public bool m_bIsReady = false;

    [SerializeField]
    private GameObject m_prefabVRStation;

    public GameObject[] m_prefabSpells;
    [SerializeField]
    private MP_VR_NetworkHand m_mpvrhand1;
    [SerializeField]
    private MP_VR_NetworkHand m_mpvrhand2;
    [SerializeField]
    private Transform hand1Spawn;
    [SerializeField]
    private Transform hand2Spawn;

    public Transform m_transMainHand;
    public Transform m_transOffHand;
    private int m_iMainHand = 0;

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
            if (m_magicwandThis.IsWandLoaded())
            {
                //1. grab the spellindex from the wands spelltype enum
                int spellIndex = (int)m_magicwandThis.LoadedSpell;  
                
                //2. Grab spelldata from loaded spell
                Spell.SpellData spelldata = m_magicwandThis.spells[spellIndex].GetSpellData(m_magicwandThis.m_SpawnPoint, m_forcerecThis.m_v3velocity);
                //3. Find out which hand is your wand hand which is your offhand
                FindMainHand();
                //4. Let the Server fire his version of the spell first
                CmdFireSpell(spelldata, spellIndex, m_iMainHand);
                //5. Now Fire the Client version of the spell
                GameObject goClient = m_magicwandThis.spells[spellIndex].Fire(m_magicwandThis.m_SpawnPoint, m_forcerecThis.m_v3velocity);
                //6. Now if you want to parent the spell to the offhand (Should be replaced with casting into the lefthand) do that
                if (spelldata._bParentToOffhand)
                {
                    Transform transOffhand = m_transOffHand.GetComponent<MP_VR_NetworkHand>().m_transVRHand;
                    goClient.transform.position = transOffhand.position;
                    goClient.transform.rotation = transOffhand.rotation;
                    FixedJoint fixJOffhand = transOffhand.GetComponent<FixedJoint>();
                    if (fixJOffhand.connectedBody != null)
                        Destroy(fixJOffhand.connectedBody.gameObject);
                    fixJOffhand.connectedBody = goClient.GetComponent<Rigidbody>();
                }
                //Last unload the wand
                m_magicwandThis.LoadWand(SpellType.NONE);
            }
        }
    }

    [Command] //Command is called on client and executed on the server
    void CmdFireSpell(Spell.SpellData _spellData, int _spellIndex, int _handIndex)
    {
        Debug.Log("Server PewPew!");
        GameObject goSpell = Instantiate<GameObject>(m_prefabSpells[_spellIndex]);
        Rigidbody rigidSpell = goSpell.GetComponent<Rigidbody>();
        if (_spellData._bParentToOffhand)
        {
            Transform offhand;
            if(_handIndex == 1)
            {
                offhand = m_mpvrhand2.transform;
            }
            if(_handIndex == 2)
                offhand = m_mpvrhand1.transform;


            goSpell.transform.position = offhand.position;
            goSpell.transform.rotation = offhand.rotation;
            FixedJoint fixJOffhand = offhand.GetComponent<FixedJoint>();
            if (fixJOffhand.connectedBody != null)
                Destroy(fixJOffhand.connectedBody.gameObject);
            fixJOffhand.connectedBody = rigidSpell;
        }         
        else
        {
            goSpell.transform.position = _spellData._v3Position;
            goSpell.transform.rotation = _spellData._qRotation;
        }
        rigidSpell.velocity = _spellData._v3Velocity;
        // Spawn the spellObject on the Clients
        NetworkServer.Spawn(goSpell);
        if(_spellData._fKillDelay > 0.0f)
            Destroy(goSpell, _spellData._fKillDelay);
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
        {
            return;
        }

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

    private void FindMainHand()
    {
        if (m_magicwandThis != null)
        {
            Valve.VR.InteractionSystem.Hand wandHand = m_magicwandThis.GetComponentInParent<Valve.VR.InteractionSystem.Hand>();
            if (m_handRight == wandHand)
            {
                m_transMainHand = m_mpvrhand1.transform;
                m_transOffHand = m_mpvrhand2.transform;
                m_iMainHand = 1;
            }
            else if(m_handLeft == wandHand) 
            {
                m_transMainHand = m_mpvrhand2.transform;
                m_iMainHand = 2;
                m_transOffHand = m_mpvrhand1.transform;
            }
            else
            {
                Debug.Log("wtf");
            }
        }
    }
}
