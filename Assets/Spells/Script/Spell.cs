using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using gesture;

public abstract class Spell: MonoBehaviour
{
    [SerializeField]
    protected GameObject m_goLoadedFX;
    [SerializeField]
    private SpellType m_spelltypeThis;
    [SerializeField]
    private gestureTypes m_gestureTypeThis;
    public SpellType SpellType { get { return m_spelltypeThis; } }

    public abstract GameObject Fire(Transform _transEndpoint, Vector3 _v3Velocity);
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
