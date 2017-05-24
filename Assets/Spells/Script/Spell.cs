using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using gesture;

public abstract class Spell : MonoBehaviour
{
    public struct SpellData
    {
        //WandData
        public Vector3 _v3WandPos;
        public Quaternion _qWandRot;
        public Vector3 _v3WandVelocity;

        //HandsData
        public int _iCastingHandIndex;

        //PlayerData
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

    protected Transform m_transTarget;
    public Transform TargetTransform { get { return m_transTarget; } }

    public abstract GameObject Fire(Transform _transEndpoint, Vector3 _v3Velocity);
    public abstract void Fire(SpellData spelldata);
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
