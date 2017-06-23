using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorSpawner : Spell
{
    public Meteor m_meteorChild;
    public Transform m_transPortal;
    public float m_fActivationRange;


    private Rigidbody m_rigidThis;
    public GameObject explosionPrefab;
    public float m_fVelocityMultiplier = 2.0f;

    CastingData m_spelldata;
    public AnimationCurve m_animcurvePortalSize;
    bool m_bFired;

    private float m_fPortalSpawnTime = 2.0f;

    public override void Deactivate()
    {
        Destroy(gameObject);
    }

    public override void Fire(CastingData spelldata)
    {
        Debug.Log("Spell: Meteooor!");
        gameObject.transform.position = spelldata._v3WandPos;
        gameObject.transform.rotation = spelldata._qWandRot;
        m_rigidThis = GetComponent<Rigidbody>();
        m_rigidThis.velocity = (spelldata._v3WandVelocity * m_fVelocityMultiplier);
        m_spelldata = spelldata;
        m_transTarget = spelldata._goPlayer.GetComponent<HomingTarget>().transform;
        m_bFired = true;
    }

    public override void PlayerHit(GameObject _goPlayer)
    {
        throw new NotImplementedException();
    }

    public override void SpellHit()
    {
        throw new NotImplementedException();
    }

    // Use this for initialization
    public override void Awake()
    {
        if (m_meteorChild == null)
            m_meteorChild = GetComponent<Meteor>();
    }

    // Update is called once per frame
    public override void Update()
    {
        if(m_bFired)
        {
            if ((m_rigidThis.transform.position - m_transTarget.position).magnitude <= m_fActivationRange)
            {
                SpawnPortal();
            }
        }
    }

    IEnumerator SpawnPortal()
    {
        bool bFire = false;
        float delta = 0.0f;
        while(delta < 1.0f)
        {
            delta += Time.deltaTime / m_fPortalSpawnTime;
            float curve = m_animcurvePortalSize.Evaluate(delta);
            m_transPortal.localScale = new Vector3(curve,curve,curve);
        }
        m_transPortal.LookAt(m_transTarget);
        if(bFire)
        {
            m_meteorChild.Fire(m_spelldata);
        }
        
        yield return null;
    }
}