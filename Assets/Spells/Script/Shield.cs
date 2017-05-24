using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : Spell
{
    //private Rigidbody m_rigidThis;

    public override void Deactivate()
    {
        throw new NotImplementedException();
    }

    public override GameObject Fire(Transform _transEndpoint, Vector3 _v3Velocity)
    {
        Debug.Log("Client PewPew!");
        GameObject goThis = Instantiate<GameObject>(m_goClientPrefab);
        goThis.transform.position = _transEndpoint.position;
        goThis.transform.rotation = _transEndpoint.rotation;
        //m_rigidThis = goThis.GetComponent<Rigidbody>();
        return goThis;
    }

    public override SpellData GetSpellData(Transform _transSpawnTransform, Vector3 _v3Velocity)
    {
        SpellData ownData;
        ownData._v3Position = _transSpawnTransform.position;
        ownData._qRotation = _transSpawnTransform.rotation;
        ownData._v3Velocity = Vector3.zero;
        ownData._bParentToOffhand = true;
        ownData._fKillDelay = 0.0f;
        return ownData;
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
