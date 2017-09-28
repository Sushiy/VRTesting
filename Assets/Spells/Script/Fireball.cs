using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : Spell
{
    private Rigidbody m_rigidThis;
    private int m_iDamage = 5;
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
    }

    public override void Fire(CastingData spelldata)
    {
        //Debug.Log("Spell: Fireball!");
        thisCastingData = spelldata;
        gameObject.transform.position = spelldata._v3WandPos;
        gameObject.transform.rotation = spelldata._qWandRot;
        m_rigidThis = GetComponent<Rigidbody>();
        m_rigidThis.velocity = (spelldata._v3WandVelocity * m_fVelocityMultiplier);
        IPlayerController player = spelldata._goPlayer.GetComponent<IPlayerController>();
        m_v3Target = player.GetTargetPosition();
        Invoke("Deactivate", 5.0f);
    }

    public void OnCollisionEnter(Collision collision)
    {
        bool destroy = false;
        //Debug.Log(collision.collider.gameObject.ToString());
        Vector3 normal = collision.contacts[0].normal;

        GameObject goOther = collision.gameObject;
        bool hitSelf = goOther.transform.root.gameObject == thisCastingData._goPlayer ? true : false;

        if (collision.gameObject.layer == LayerMask.NameToLayer("Shield") && absorptionPrefab != null)
        {
            GameObject absorbtion = Instantiate(absorptionPrefab, transform.position, transform.rotation);
            absorbtion.transform.forward = normal;
            destroy = true;
        }
        else if (explosionPrefab != null && !hitSelf)
        {
            Instantiate(explosionPrefab, transform.position, transform.rotation);
            destroy = true;
        }

        if (goOther.layer == LayerMask.NameToLayer("Player") && !hitSelf)
        {
            PlayerHit(goOther);
        }

        if(destroy)
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
