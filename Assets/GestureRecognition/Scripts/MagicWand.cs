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

    private SpellType m_enumLoadedSpell = SpellType.NONE;
    private GameObject m_loadedfx;
    public SpellType LoadedSpell { get { return m_enumLoadedSpell; } }
    public bool isMainHand;// { get; private set; }

    void Awake()
    {
        Assert.IsNotNull(m_SpawnPoint);
        Assert.IsNotNull(spellRegistry);
    }

    void Update()
    {
    }

    public void LoadWand(SpellType spell)
    {
        m_enumLoadedSpell = spell;
    }

    public void UnLoadWand()
    {
        if(m_loadedfx != null)
        {
            GameObject go = m_loadedfx;
            Destroy(go);
            m_loadedfx = null;
        }
        m_enumLoadedSpell = SpellType.NONE;
    }

    public void LoadWand(gesture.gestureTypes _gesture)
    {
        foreach(GameObject go in spellRegistry.clientPrefabs)
        {
            if(go!=null)
            {
                Spell s = go.GetComponent<Spell>();
                if (s != null && s.Gesture == _gesture)
                {
                    m_enumLoadedSpell = s.SpellType;
                    if (s.LoadedFX() != null)
                        m_loadedfx = GameObject.Instantiate<GameObject>(s.LoadedFX(), m_SpawnPoint.position, m_SpawnPoint.rotation, m_SpawnPoint);
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
