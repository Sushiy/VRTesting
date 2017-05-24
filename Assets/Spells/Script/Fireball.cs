using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : Spell
{
    private Rigidbody m_rigidThis;
    private int m_iDamage = 10;
    public GameObject explosionPrefab;

    // Use this for initialization
    public override void Awake()
    {
        base.Awake();
    }

    // Update is called once per frame
    public override void Update()
    {

    }

    public override void Deactivate()
    {
        throw new NotImplementedException();
    }

    public override GameObject Fire(Transform _transEndpoint, Vector3 _v3Velocity)
    {
        Debug.Log("Client PewPew!");
        GameObject goThis = Instantiate<GameObject>(m_goClientPrefab);
        goThis.transform.position = _transEndpoint.position;
        m_rigidThis = goThis.GetComponent<Rigidbody>();
        m_rigidThis.velocity = (_v3Velocity * 3.0f);
        Destroy(goThis, 5.0f);
        return goThis;
    }

    public override void Fire(SpellData spelldata)
    {
        Debug.Log("Spell: Fire!");
        gameObject.transform.position = spelldata._v3WandPos;
        gameObject.transform.rotation = spelldata._qWandRot;
        m_rigidThis = GetComponent<Rigidbody>();
        m_rigidThis.velocity = (spelldata._v3WandVelocity * 3.0f);
        MP_VR_PlayerController player = spelldata._goPlayer.GetComponent<MP_VR_PlayerController>();
        if (player.Opponent != null)
            m_transTarget = player.Opponent.transform;
        Destroy(gameObject, 5.0f);
    }

    public void OnCollisionEnter(Collision collision)
    {
        Instantiate(explosionPrefab, transform.position, transform.rotation);
        GameObject goOther = collision.gameObject;
        if (goOther.layer == LayerMask.NameToLayer("Player"))
        {
            goOther.GetComponentInParent<MP_Health>().TakeDamage(m_iDamage);
        }
        Destroy(gameObject);
    }

    public override void PlayerHit()
    {
        throw new NotImplementedException();
    }

    public override void SpellHit()
    {
        throw new NotImplementedException();
    }
}
