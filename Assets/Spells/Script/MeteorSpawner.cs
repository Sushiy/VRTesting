using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorSpawner : Spell
{
    public Fireball m_fireballChild;
    public float m_factivationRange;


    private Rigidbody m_rigidThis;
    public GameObject explosionPrefab;
    public float m_fVelocityMultiplier = 2.0f;

    CastingData m_spelldata;
    Transform m_transTarget;
    bool m_bFired;

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
    public override void Start()
    {

    }

    // Update is called once per frame
    public override void Update()
    {
        if(m_bFired)
        {
            if ((m_rigidThis.transform.position - m_transTarget.position).magnitude <= m_factivationRange)
            {
                m_fireballChild.Fire(m_spelldata);
            }
        }
    }
}