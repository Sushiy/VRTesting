using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public enum SpellType
{
    NONE,
    FIREBALL,
    SHIELD,
    MAGICMISSILE,
    METEOR,
    LASERBEAM
}

public class MagicWand : MonoBehaviour {

    [SerializeField]
    private SpellRegistry spellRegistry;
    [SerializeField]
    public Transform m_SpawnPoint;
    [SerializeField]
    private SpellType m_enumLoadedSpell = SpellType.NONE;
    private SpellType m_enumLastSpell = SpellType.NONE;
    private GameObject m_loadedfx;
    public SpellType LoadedSpell { get { return m_enumLoadedSpell; } }
    public bool isMainHand;// { get; private set; }

    public float offHandCastTimer = 8.0f;
    private float timer = 0.0f;
    public bool hasCasted = false;
    public ParticleSystem chargingParticles;
    void Awake()
    {
        Assert.IsNotNull(m_SpawnPoint);
        Assert.IsNotNull(spellRegistry);
        LoadWand(m_enumLoadedSpell);
    }

    void Update()
    {
        if(!isMainHand && hasCasted)
        {
            if (!chargingParticles.isPlaying)
                chargingParticles.Play();
            timer += Time.deltaTime;
            if (timer > offHandCastTimer)
            {
                hasCasted = false;
                LoadWand(m_enumLastSpell);
                timer = 0.0f;
                chargingParticles.Stop();
            }
        }
    }

    public void UnLoadWand()
    {
        Debug.Log("Unload Wand");
        m_enumLastSpell = m_enumLoadedSpell;
        if(m_loadedfx != null)
        {
            GameObject go = m_loadedfx;
            Destroy(go);
            m_loadedfx = null;
        }
        m_enumLoadedSpell = SpellType.NONE;
    }

    public void LoadWand(SpellType spell)
    {
        foreach(GameObject go in spellRegistry.clientPrefabs)
        {
            if(go!=null)
            {
                Spell s = go.GetComponent<Spell>();
                if (s != null && s.SpellType == spell)
                {
                    m_enumLoadedSpell = s.SpellType;
                    if (s.LoadedFX() != null)
                    {
                        GameObject ps = m_loadedfx;
                        Destroy(ps);
                        m_loadedfx = GameObject.Instantiate<GameObject>(s.LoadedFX(), m_SpawnPoint.position, m_SpawnPoint.rotation, m_SpawnPoint);
                    }
                }

            }
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
