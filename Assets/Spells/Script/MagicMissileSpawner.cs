using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicMissileSpawner : SpawnerSpell
{
    public int m_iChildrenDestroyed = 0;

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
        //Invoke("Deactivate", 10.0f);
    }

    public override void ChildDestroyed()
    {
        m_iChildrenDestroyed++;
        if (m_iChildrenDestroyed == 3)
            Deactivate();
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
