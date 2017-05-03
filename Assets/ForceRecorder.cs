﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class ForceRecorder : MonoBehaviour {

    [SerializeField]
    private float m_fMaximumVelocity = 100f;
    [SerializeField]
    private float m_fVelocityThreshold = 70f;
    [SerializeField]
    private float m_fTimeBetweenFlicks = 0.1f;
    [SerializeField]
    private MagicWand m_MagicWand;

    private float m_fHighestValue = 0;
    private float m_fTimer = 0f;

    private Rigidbody m_rigid;
    private ParticleSystem m_flickParticles;
    
    void Awake()
    {
        m_rigid = GetComponent<Rigidbody>();
        Assert.IsNotNull<MagicWand>(m_MagicWand);

        m_flickParticles = GetComponentInChildren<ParticleSystem>();
        Assert.IsNotNull<ParticleSystem>(m_flickParticles);
    }

	void FixedUpdate()
    {
        // check if velocity is high enough
        float val = m_rigid.velocity.sqrMagnitude;
        if (val > m_fHighestValue)
        {
            m_fHighestValue = val;
            //print("New Highest Value: " + val);
        }

        // prevent that flicks get detected to often in one motion
        if (m_fTimer > 0f)
        {
            m_fTimer -= Time.deltaTime;
            if (m_fTimer <= 0.005f) m_fTimer = 0f;
        }

        // is the velocity high enough and time has passed?
        if (val > m_fVelocityThreshold && m_fTimer <= 0.005f)
        {
            FlickDetected();
            m_fTimer = m_fTimeBetweenFlicks;
        }
    }

    // function gets called once a flick is detected
    void FlickDetected()
    {
        if (m_flickParticles.isPlaying)
            m_flickParticles.Stop();
        m_flickParticles.transform.position = transform.position;
        m_flickParticles.Play();
        m_MagicWand.FireSpell(m_rigid.velocity);
    }
}
