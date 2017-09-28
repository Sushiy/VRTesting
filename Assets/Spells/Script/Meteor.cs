using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteor : Spell
{
    private Rigidbody m_rigidThis;
    private int m_iDamage = 20;
    public GameObject explosionPrefab;
    public GameObject absorptionPrefab;
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
        transform.parent.GetComponent<MeteorSpawner>().ChildDestroyed();
    }

    public override void Fire(CastingData spelldata)
    {
        //Debug.Log("Spell: Fire!");
        IPlayerController player = spelldata._goPlayer.GetComponent<IPlayerController>();
        Vector3 targetPosition = player.GetTargetPosition();
        gameObject.transform.position = spelldata._v3WandPos;
        m_rigidThis = GetComponent<Rigidbody>();
        m_rigidThis.velocity = ((targetPosition - transform.position) * m_fVelocityMultiplier);
    }

    public void OnCollisionEnter(Collision collision)
    {
        Vector3 normal = collision.contacts[0].normal;
        if (collision.gameObject.layer == LayerMask.NameToLayer("Shield") && absorptionPrefab != null)
        {
            GameObject absorbtion = Instantiate(absorptionPrefab, transform.position, transform.rotation);
            absorbtion.transform.forward = normal;
        }
        else if (explosionPrefab != null)
            Instantiate(explosionPrefab, transform.position, transform.rotation);
        GameObject goOther = collision.gameObject;
        
        if (m_bIsServer && goOther.layer == LayerMask.NameToLayer("Player"))
        {
            PlayerHit(goOther);
        }
        Deactivate();
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