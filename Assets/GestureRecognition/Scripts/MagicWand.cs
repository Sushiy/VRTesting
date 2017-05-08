using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public enum SpellType
{
    FIREBALL, NONE
}

public class MagicWand : MonoBehaviour {

    [SerializeField]
    private float m_fVelocityMultiplier = 3f;
    [SerializeField]
    public GameObject prefab_Fireball;
    [SerializeField]
    public Transform m_SpawnPoint;

    private SpellType m_enumLoadedSpell = SpellType.FIREBALL;
    private ParticleSystem m_loadedParticles;
    public SpellType LoadedSpell { get { return m_enumLoadedSpell; } }

    void Awake()
    {
        Assert.IsNotNull<Transform>(m_SpawnPoint);
        Assert.IsNotNull<GameObject>(prefab_Fireball);
        m_loadedParticles = GetComponentInChildren<ParticleSystem>();
        Assert.IsNotNull<ParticleSystem>(m_loadedParticles);
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

    public void FireSpell(Vector3 velocity)
    {
        if (m_enumLoadedSpell == SpellType.NONE) return;

        if (m_enumLoadedSpell == SpellType.FIREBALL)
        {
            GameObject fireball = Instantiate<GameObject>(prefab_Fireball);
            fireball.transform.position = m_SpawnPoint.position;
            fireball.GetComponent<Rigidbody>().velocity = (velocity * m_fVelocityMultiplier);
        }

        m_enumLoadedSpell = SpellType.NONE;
    }

    public bool IsWandLoaded()
    {
        return m_enumLoadedSpell != SpellType.NONE;
    }

    public bool isWandLoaded(SpellType type)
    {
        return m_enumLoadedSpell == type;
    }
}
