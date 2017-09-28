using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Valve.VR.InteractionSystem;

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
    private Hand hand;
    public bool m_bVibrateOn = true;
    [Range(1f,3999f)]
    public float m_fMaxAmplitudeVibration = 1000f;
    [Range(1f,10f)]
    public float m_fVibrationFreq = 2f;

    private Coroutine vibrate_coroutine = null;

    void Awake()
    {
        Assert.IsNotNull(m_SpawnPoint);
        Assert.IsNotNull(spellRegistry);
        if (!isMainHand)
            LoadWand(m_enumLoadedSpell);
        hand = transform.parent.GetComponent<Hand>();
        Assert.IsNotNull(hand);
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
        m_enumLastSpell = m_enumLoadedSpell;
        if(m_loadedfx != null)
        {
            GameObject go = m_loadedfx;
            Destroy(go);
            m_loadedfx = null;
        }
        m_enumLoadedSpell = SpellType.NONE;

        if (vibrate_coroutine != null)
        {
            StopCoroutine(vibrate_coroutine);
            vibrate_coroutine = null;
        }
    }

    public void LoadWand(SpellType spell)
    {
        // if the offhand is charging right now and the
        // current spell gets overwritten, only change the
        // "lastSpell"
        if (!isMainHand && hasCasted)
        {
            m_enumLastSpell = spell;
            return;
        }

        foreach (GameObject go in spellRegistry.clientPrefabs)
        {
            if(go!=null)
            {
                Spell s = go.GetComponent<Spell>();
                if (s != null && s.SpellType == spell)
                {
                    // load the wand
                    m_enumLoadedSpell = s.SpellType;
                    vibrate_coroutine = StartCoroutine(VibrateLoadedCoroutine());

                    // notify the tutorial
                    if (isMainHand && GestureTutorial.s_instance != null) GestureTutorial.s_instance.WandLoaded(m_enumLoadedSpell);

                    // start the loaded fx particlesystem
                    if (s.LoadedFX() != null)
                    {
                        GameObject ps = m_loadedfx;
                        Destroy(ps);
                        m_loadedfx = Instantiate(s.LoadedFX(), m_SpawnPoint.position, m_SpawnPoint.rotation, m_SpawnPoint);
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

    /**
     * Gets started when Wand gets loaded and stopped when wand unloads
     * Periodically (sinus curve) lets the controller vibrate.
     */
    private IEnumerator VibrateLoadedCoroutine()
    {
        while (true)
        {
            float halfAmplitude = m_fMaxAmplitudeVibration / 2f;
            float pulse = Mathf.Cos(Time.time * m_fVibrationFreq) * halfAmplitude + halfAmplitude;

            if (hand != null && m_bVibrateOn)
                hand.controller.TriggerHapticPulse((ushort) Mathf.RoundToInt(pulse));
            yield return null;
        }
    }
}
