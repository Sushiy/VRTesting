using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using gesture;

public abstract class Spell : MonoBehaviour
{
    public struct SpellData
    {
        public Vector3 _v3Position;
        public Quaternion _qRotation;
        public Vector3 _v3Velocity;
        public bool _bParentToHand;
        public float _fKillDelay;
    }

    public struct SpellData2
    {
        //WandData
        public Vector3 _v3WandPos;
        public Quaternion _qWandRot;
        public Vector3 _v3WandVelocity;

        //HandsData
        public GameObject _goPlayer;
    }

    [SerializeField]
    protected GameObject m_goLoadedFX;
    [SerializeField]
    protected GameObject m_goClientPrefab;
    [SerializeField]
    protected SpellType m_spelltypeThis;
    public SpellType SpellType { get { return m_spelltypeThis; } }
    [SerializeField]
    protected gestureTypes m_gestureTypeThis;
    public gestureTypes Gesture{ get { return m_gestureTypeThis; } }

    protected Vector3 m_v3TargetPos;
    public Vector3 TargetPosition { get { return m_v3TargetPos; } }

    public abstract GameObject Fire(Transform _transEndpoint, Vector3 _v3Velocity);
    public abstract void Fire(SpellData2 spelldata);
    public abstract SpellData GetSpellData(Transform _transSpawnTransform, Vector3 _v3Velocity);
    public abstract void PlayerHit();
    public abstract void SpellHit();
    public abstract void Deactivate();
    public GameObject Loaded()
    {
        return m_goLoadedFX;
    }

    public virtual void Awake()
    {

    }

    public virtual void Start()
    {

    }

    public virtual void Update()
    {

    }
}
