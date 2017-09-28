using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Valve.VR;

public class MP_VR_PlayerController : NetworkBehaviour, IPlayerController
{

    public bool notSpawned = false;

    [SerializeField]
    private MP_VR_PlayerController m_mpvr_playerOpponent;
    public MP_VR_PlayerController Opponent { get { return m_mpvr_playerOpponent; } }
    public bool m_bIsReady = false;

    private Vector3 targetPosition;

    public MP_VR_NetworkHMD head;

    public Vector3 GetTargetPosition()
    {
        if (Opponent != null)
            return Opponent.head.transform.position;// + new Vector3(0, 2.0f, 0);
        else
            return targetPosition;
    }

    [SerializeField]
    private GameObject m_prefabVRStation;

    public SpellRegistry spellregistry;
    [SerializeField]
    private GameObject m_prefabNetworkWand;

    [SerializeField]
    private MP_VR_NetworkHand m_mpvrhand1;
    [SerializeField]
    private MP_VR_NetworkHand m_mpvrhand2;
    [SerializeField]
    private Transform hand1Spawn;
    [SerializeField]
    private Transform hand2Spawn;
    [SerializeField]
    private Valve.VR.InteractionSystem.Player m_vrplayerThis;

    public Valve.VR.InteractionSystem.Player GetVRPlayer()
    {
        return m_vrplayerThis;
    }

    private Valve.VR.InteractionSystem.Hand m_handRight;
    private Valve.VR.InteractionSystem.Hand m_handLeft;

    //Mainhand Wand and forceRecorder
    private ForceRecorder m_forcerecMain;
    private MagicWand m_magicwandMain;

    //Offhand Wand and forceRecorder
    private ForceRecorder m_forcerecOff;
    private MagicWand m_magicwandOff;

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
        //Try to find your opponent
        CmdFindOpp(gameObject);
        //if you couldn't find your opponent shoot the default location
        if(Opponent == null)
            targetPosition = transform.position + transform.forward * 32.0f;

        head = GetComponentInChildren<MP_VR_NetworkHMD>();
    }

    public void Start()
    {
        //targetPosition = transform.position + transform.forward * 32.0f;
        Debug.Log("start called");
        if (isLocalPlayer)
        {
            Debug.Log("...on not server");

        }
    }

    // Update is called once per frame
    void Update ()
    {
        //Check if you are a local Player
        if (!isLocalPlayer)
        {
            return;
        }
        //Check if you have an opponent
        if (m_mpvr_playerOpponent == null)
        {
            CmdFindOpp(gameObject);
        }

        //Check if you have found your hands
        if (m_handLeft == null || m_handRight == null)
        {
            m_bIsReady = false;
            CheckHands();
            return;
        }

        //Check if you have found your ForceRecorder
        if (m_forcerecMain == null)
        {
            m_bIsReady = false;
            InitSpellComponents();
            return;
        }

        //if you weren't ready yet and made it this far, get ready
        if(!m_bIsReady)
            m_bIsReady = true;
        
        //If the forcerecorder wants us to fire spells, do it
        if(m_forcerecMain.isFiring())
        {
            if (m_magicwandMain.IsWandLoaded())
            {
                CastASpell(m_magicwandMain);
            }
        }

        //If the forcerecorder wants us to fire spells, do it
        if (m_forcerecOff.isFiring())
        {
            if (m_magicwandOff.IsWandLoaded())
            {
                m_magicwandOff.hasCasted = true;
                CastASpell(m_magicwandOff);
            }
        }
    }

    void CastASpell(MagicWand _magicwand)
    {
        ForceRecorder forceRec = (_magicwand.isMainHand) ? m_forcerecMain : m_forcerecOff;

        //1. grab the spellindex from the wands spelltype enum
        int spellIndex = (int)_magicwand.LoadedSpell;
        
        //2. Find out which hand is your wand hand which is your offhand
        int iCastingHandIndex = FindWandHand(_magicwand);
        //3. Let the Server fire his version of the spell first
        CmdServerFireSpell(forceRec.m_v3velocity, _magicwand.m_SpawnPoint.position, _magicwand.m_SpawnPoint.rotation, spellIndex, iCastingHandIndex, gameObject);
        //Last unload the wand
        _magicwand.UnLoadWand();
    }
    [ClientRpc]
    void RpcClientFireSpell(Vector3 velocity, Vector3 spawnPosition, Quaternion spawnRotation, int _spellIndex, int _castingHandIndex)
    {
        GameObject goClient = Instantiate<GameObject>(spellregistry.clientPrefabs[_spellIndex]);
        Spell.CastingData spelldata = new Spell.CastingData();
        spelldata._v3WandPos = spawnPosition;
        spelldata._qWandRot = spawnRotation;
        spelldata._v3WandVelocity = velocity;
        spelldata._iCastingHandIndex = _castingHandIndex;
        spelldata._goPlayer = gameObject;
        Spell spell = goClient.GetComponent<Spell>();
        spell.Fire(spelldata);
    }

    [Command]
    void CmdServerFireSpell(Vector3 velocity, Vector3 spawnPosition, Quaternion spawnRotation, int _spellIndex, int _castingHandIndex, GameObject _playerThis)
    {
        //GameObject goServer = Instantiate<GameObject>(m_prefabServerSpells[_spellIndex]);
        GameObject goServer = Instantiate<GameObject>(spellregistry.serverPrefabs[_spellIndex]);
        Spell.CastingData spelldata = new Spell.CastingData();
        spelldata._v3WandPos = spawnPosition;
        spelldata._qWandRot = spawnRotation;
        spelldata._v3WandVelocity = velocity;
        spelldata._iCastingHandIndex = _castingHandIndex;
        spelldata._goPlayer = _playerThis;

        Spell spell = goServer.GetComponent<Spell>();
        spell.Fire(spelldata);
        // Spawn the spellObject on the Clients
        NetworkServer.Spawn(goServer);
        RpcClientFireSpell(velocity, spawnPosition, spawnRotation, _spellIndex, _castingHandIndex);

    }

    //Find ForceRecorder and MagicWand Components
    private void InitSpellComponents()
    {
        if (m_forcerecMain != null && m_magicwandMain != null && m_forcerecOff != null && m_magicwandOff != null)
            return;

        ForceRecorder[] forceRecorders = m_vrplayerThis.GetComponentsInChildren<ForceRecorder>();
        if (forceRecorders.Length != 2)
        {
            //Debug.LogError("There are not exactly 2 Forcerecorders on this player");
            return;
        }
        foreach(ForceRecorder f in forceRecorders)
        {
            if (f.MagicWand.isMainHand)
            {
                m_forcerecMain = f;
                m_magicwandMain = f.MagicWand;
                //CmdSpawnNetworkWand(m_prefabNetworkWand, FindWandHand(f.MagicWand), gameObject);
            }
            else
            {
                m_forcerecOff = f;
                m_magicwandOff = f.MagicWand;
            }
        }

        if(m_forcerecMain != null && m_forcerecOff != null)
        {
            m_forcerecOff.RemoveFromParent();
            m_forcerecMain.RemoveFromParent();
        }
    }


    [Command]
    private void CmdSpawnNetworkWand(GameObject _prefabWand, int _castingHandIndex, GameObject _goPlayerThis)
    {
        RpcSpawnNetworkWand(_prefabWand, _castingHandIndex, _goPlayerThis);
    }

    [ClientRpc]
    private void RpcSpawnNetworkWand(GameObject _prefabWand, int _castingHandIndex, GameObject _goPlayerThis)
    {
        GameObject wand = Instantiate<GameObject>(_prefabWand);
        MP_VR_PlayerController player = _goPlayerThis.GetComponent<MP_VR_PlayerController>();
        Transform transCastingHand = player.GetCastingHand(_castingHandIndex);
        FixedJoint joint = transCastingHand.GetComponentsInChildren<FixedJoint>()[1];
        Rigidbody rigidWand = wand.GetComponent<Rigidbody>();
        wand.transform.parent = joint.transform;
        wand.transform.position = joint.transform.position;
        wand.transform.rotation = joint.transform.rotation;
        wand.transform.rotation *= Quaternion.Euler(-90, 0, 0);
        joint.connectedBody = rigidWand;
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

    public int FindWandHand(MagicWand _magicwand)
    {
        if (_magicwand != null)
        {
            Valve.VR.InteractionSystem.Hand wandHand = _magicwand.GetComponentInParent<Valve.VR.InteractionSystem.Hand>();
            if (m_handRight == wandHand)
            {
               return 1;
            }
            else if(m_handLeft == wandHand) 
            {
                return 2;
            }
        }
        return 0;
    }

    [Command]
    public void CmdFindOpp(GameObject me)
    {
        MP_VR_NetworkManagerExtension manager = NetworkManager.singleton as MP_VR_NetworkManagerExtension;
        GameObject opp = null;
        if (manager.m_goPlayer1 == gameObject && manager.m_goPlayer2 != null)
            opp = manager.m_goPlayer2;
        else if (manager.m_goPlayer2 == gameObject && manager.m_goPlayer1 != null)
            opp = manager.m_goPlayer1;
        else
            Debug.Log("Could not find your opponent");

        if(opp != null)
        {
            RpcSetOpp(me, opp);
        }
    }
    [ClientRpc]
    public void RpcSetOpp(GameObject me, GameObject opp)
    {
        if (gameObject == me)
            SetOpponent(opp.GetComponent<MP_VR_PlayerController>());
    }


    public Transform GetCastingHand(int _iCastingHandIndex)
    {
        Transform castingHand = m_mpvrhand1.transform;
        if (_iCastingHandIndex == 1)
        {
            castingHand = m_mpvrhand1.transform;
        }
        if (_iCastingHandIndex == 2)
            castingHand = m_mpvrhand2.transform;
        return castingHand;
    }

    public void SetOpponent(MP_VR_PlayerController _opponent)
    {
        m_mpvr_playerOpponent = _opponent;
    }
}
