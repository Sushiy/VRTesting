using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicMissileSpawner : Spell
{

    public override void Deactivate()
    {
        Destroy(gameObject);
    }

    public override void Fire(CastingData spelldata)
    {
        Debug.Log("Spell: Fire!");
        gameObject.transform.position = spelldata._v3WandPos;
        gameObject.transform.rotation = spelldata._qWandRot;
        MagicMissile[] missiles = GetComponentsInChildren<MagicMissile>();
        foreach(MagicMissile mm in missiles)
        {
            if(mm != null)
            {
                mm.Fire(spelldata);
            }
        }
        Invoke("Deactivate", 5.0f);
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
    public override void Start()
    {

    }

    // Update is called once per frame
    public override void Update()
    {

    }
}
