using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorSpawner : Spell
{
    public Meteor m_meteorChild;
    public Transform m_transPortal;
    public Transform m_transPortalParent;
    public ParticleSystem m_psSpawn;
    public GameObject m_goMissile;
    public float m_fActivationRange;


    private Rigidbody m_rigidThis;
    public GameObject explosionPrefab;
    public float m_fVelocityMultiplier = 2.0f;

    CastingData m_spelldata;
    public AnimationCurve m_animcurvePortalSize;
    bool m_bFired;

    public float m_fPortalSpawnTime = 1.0f;

    public override void Deactivate()
    {
        Destroy(gameObject);
    }

    public override void Fire(CastingData spelldata)
    {
        Debug.Log("Spell: Fire!");
        gameObject.transform.position = spelldata._v3WandPos;
        gameObject.transform.rotation = spelldata._qWandRot;
        m_rigidThis = GetComponent<Rigidbody>();
        m_rigidThis.velocity = (spelldata._v3WandVelocity * m_fVelocityMultiplier);
        MP_VR_PlayerController player = spelldata._goPlayer.GetComponent<MP_VR_PlayerController>();
        if (player.Opponent != null)
            m_transTarget = player.Opponent.GetComponentInChildren<HomingTarget>().transform;

        m_spelldata = spelldata;
        m_bFired = true;

        Invoke("Deactivate", 10.0f);
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
                m_rigidThis.velocity = Vector3.zero;
                m_rigidThis.angularVelocity = Vector3.zero;
                GetComponent<PhysicsHoming>().enabled = false;
                GetComponent<ConstantForce>().enabled = false;
                m_transPortalParent.gameObject.SetActive(true);
                m_transPortalParent.LookAt(m_transTarget);
                GetComponent<MeshRenderer>().enabled = false;
                m_transPortal.GetComponent<ParticleSystem>().Play();
                
                m_psSpawn.Play();
                StartCoroutine(SpawnPortal());
                m_bFired = false;
            }
        }
    }

    IEnumerator SpawnPortal()
    {
        Debug.Log("Portal Spawned");

        float delta = 0.0f;
        while(delta < 1.0f)
        {
            delta += Time.deltaTime / m_fPortalSpawnTime;
            float curve = m_animcurvePortalSize.Evaluate(delta);
            m_transPortal.localScale = new Vector3(curve,curve,curve);
            yield return null;
        }
        m_spelldata._v3WandPos = m_rigidThis.position;

        yield return new WaitForSeconds(0.5f);
        m_goMissile.SetActive(false);
        m_meteorChild.gameObject.SetActive(true);
        m_meteorChild.Fire(m_spelldata);
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(DespawnPortal());
        yield return null;
    }

    IEnumerator DespawnPortal()
    {
        Debug.Log("Portal Despawning");
        float delta = 0.0f;
        while (delta < 1.0f)
        {
            delta += Time.deltaTime / m_fPortalSpawnTime;
            float curve = 1.0f - m_animcurvePortalSize.Evaluate(delta);
            m_transPortal.localScale = new Vector3(curve, curve, curve);
            yield return null;
        }
        m_transPortalParent.gameObject.SetActive(false);
    


        yield return null;
    }
}