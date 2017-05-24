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

    [SerializeField]
    protected GameObject m_goLoadedFX;
    [SerializeField]
    protected GameObject m_goClientPrefab;
    [SerializeField]
    private SpellType m_spelltypeThis;
    [SerializeField]
    private gestureTypes m_gestureTypeThis;
    public gestureTypes Gesture
    {
        get { return m_gestureTypeThis; }
    }
    public SpellType SpellType { get { return m_spelltypeThis; } }

    public abstract GameObject Fire(Transform _transEndpoint, Vector3 _v3Velocity);
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

    public virtual void Update()
    {

    }
}
