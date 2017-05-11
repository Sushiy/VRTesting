using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : Spell
{
    private Rigidbody m_rigidThis;

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
        GameObject goThis = Instantiate<GameObject>(gameObject);
        goThis.transform.position = _transEndpoint.position;
        m_rigidThis = goThis.GetComponent<Rigidbody>();
        m_rigidThis.velocity = (_v3Velocity * 3.0f);
        Destroy(goThis, 5.0f);
        return goThis;
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
