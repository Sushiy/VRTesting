using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : Spell
{
    //private Rigidbody m_rigidThis;
    private float timer = 0.0f;
    private static float maxShield = 4.0f;
    public AnimationCurve curve;
    public override void Deactivate()
    {
        Destroy(gameObject);
    }

    public override void Fire(CastingData spelldata)
    {
        Debug.Log("Shield: Fire");
        //Fetch the MPVRPlayerController
        IPlayerController player = spelldata._goPlayer.GetComponent<IPlayerController>();
        //Get the transform of the currently casting hand
        Transform transCastingHand = player.GetCastingHand(spelldata._iCastingHandIndex);
        //Get rigidbody and the fixedjoint of the casting hand
        Rigidbody rigidSpell = GetComponent<Rigidbody>();
        transform.position = transCastingHand.position;
        transform.rotation = transCastingHand.rotation;
        transform.rotation *= Quaternion.Euler(0, 90, 0);
        transform.position += transform.right * -0.7f;
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
        timer += Time.deltaTime/maxShield;
        MeshRenderer m = GetComponent<MeshRenderer>();

        m.material.SetFloat("_SliceAmount", curve.Evaluate(timer));
    }
}
