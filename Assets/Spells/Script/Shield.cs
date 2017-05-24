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

    public override void Fire(SpellData spelldata)
    {
        Debug.Log("Shield: Fire");
        //Fetch the MPVRPlayerController
        MP_VR_PlayerController player = spelldata._goPlayer.GetComponent<MP_VR_PlayerController>();
        //Get the transform of the currently casting hand
        Transform transCastingHand = player.GetCastingHand(spelldata._iCastingHandIndex);
        //Get rigidbody and the fixedjoint of the casting hand
        Rigidbody rigidSpell = GetComponent<Rigidbody>();
        transform.position = transCastingHand.position;
        transform.rotation = transCastingHand.rotation;
        FixedJoint fixJOffhand = transCastingHand.GetComponent<FixedJoint>();
        if (fixJOffhand.connectedBody != null)
            Destroy(fixJOffhand.connectedBody.gameObject);
        fixJOffhand.connectedBody = rigidSpell;

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
