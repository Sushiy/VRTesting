using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public enum SpellType
{
    NONE,
    FIREBALL,
    SHIELD
}

public class MagicWand : MonoBehaviour {

    //[SerializeField]
    //private float m_fVelocityMultiplier = 3f;
    [SerializeField]
    public Spell[] spells;
    [SerializeField]
    public Transform m_SpawnPoint;

    private SpellType m_enumLoadedSpell = SpellType.FIREBALL;
    private ParticleSystem m_loadedParticles;
    public SpellType LoadedSpell { get { return m_enumLoadedSpell; } }
    public bool isMainHand;// { get; private set; }

    void Awake()
    {
        Assert.IsNotNull(m_SpawnPoint);
        Assert.IsNotNull(spells);
        m_loadedParticles = GetComponentInChildren<ParticleSystem>();
        Assert.IsNotNull<ParticleSystem>(m_loadedParticles);

        //isMainHand = false;
    }

    void Update()
    {
        // is loaded?
        if (m_enumLoadedSpell != SpellType.NONE)
        {
            // play the particle effect
            if (!m_loadedParticles.isPlaying) m_loadedParticles.Play();
        }
        // not loaded
        else
        {
            if (m_loadedParticles.isPlaying) m_loadedParticles.Stop();
        }
    }

    public void LoadWand(SpellType spell)
    {
        m_enumLoadedSpell = spell;
    }

    public void LoadWand(gesture.gestureTypes _gesture)
    {
        foreach(Spell s in spells)
        {
            if (s != null && s.Gesture == _gesture)
                m_enumLoadedSpell = s.SpellType;
        }
    }

    public bool IsWandLoaded()
    {
        return m_enumLoadedSpell != SpellType.NONE;
    }

    
    public bool isWandLoaded(SpellType type)
    {
        return m_enumLoadedSpell == type;
    }

    public void setMainHand(bool b)
    {
        isMainHand = b;
    }
}
