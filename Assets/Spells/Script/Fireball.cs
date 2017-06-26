using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : Spell
{
    private Rigidbody m_rigidThis;
    private int m_iDamage = 5;
    public GameObject explosionPrefab;
    public float m_fVelocityMultiplier = 2.0f;

    // Use this for initialization
    public override void Awake()
    {
        base.Awake();
    }

    public override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    public override void Update()
    {

    }

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
        Invoke("Deactivate", 5.0f);
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (explosionPrefab != null)
            Instantiate(explosionPrefab, transform.position, transform.rotation);

        GameObject goOther = collision.gameObject;
        if (m_bIsServer && goOther.layer == LayerMask.NameToLayer("Player"))
        {
            PlayerHit(goOther);
        }
        Destroy(gameObject);
    }

    public override void PlayerHit(GameObject _goPlayer)
    {
        _goPlayer.GetComponentInParent<MP_Health>().TakeDamage(m_iDamage);
    }

    public override void SpellHit()
    {
        throw new NotImplementedException();
    }
}
