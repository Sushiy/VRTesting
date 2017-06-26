using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : Spell
{
    //private Rigidbody m_rigidThis;
    private float timer = 0.0f;
    private static float maxShield = 3.0f;
    public AnimationCurve curve;
    public override void Deactivate()
    {
        Destroy(gameObject);
    }

    public override void Fire(CastingData spelldata)
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
        if (spelldata._iCastingHandIndex == 1)
            transform.rotation *= Quaternion.Euler(0, 0, 180);
        FixedJoint fixJHand = transCastingHand.GetComponent<FixedJoint>();
        if (fixJHand.connectedBody != null)
            Destroy(fixJHand.connectedBody.gameObject);
        fixJHand.connectedBody = rigidSpell;

        Invoke("Deactivate", maxShield);

    }

    public override void PlayerHit(GameObject _goPlayer)
    {
        throw new NotImplementedException();
    }

    public override void SpellHit()
    {
        throw new NotImplementedException();
    }

    public override void Update()
    {
        timer += Time.deltaTime;
        MeshRenderer m = GetComponent<MeshRenderer>();

        m.material.SetFloat("_SliceAmount", curve.Evaluate(timer));
    }
}
