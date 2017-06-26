using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicMissile : Spell
{
    private Rigidbody m_rigidThis;
    private int m_iDamage = 4;
    public GameObject explosionPrefab;

    public float m_fVelocityMultiplier = 4.0f;
    public float m_fMinAngle = 15.0f;
    public float m_fMaxAngle = 30.0f;

    private Quaternion m_qRandDir = Quaternion.identity;

    public override void Deactivate()
    {
        transform.parent.GetComponent<MagicMissileSpawner>().ChildDestroyed();
        Destroy(gameObject);
    }

    public override void Fire(CastingData spelldata)
    {
        Debug.Log("Spell: Fire!");
        gameObject.transform.position = spelldata._v3WandPos;
        gameObject.transform.rotation = spelldata._qWandRot;
        m_rigidThis = GetComponent<Rigidbody>();
        if(m_qRandDir == Quaternion.identity)
            MakeRandomDirection();
        Vector3 randomizedDir = m_qRandDir  * spelldata._v3WandVelocity;
        m_rigidThis.velocity = (randomizedDir * m_fVelocityMultiplier);
        MP_VR_PlayerController player = spelldata._goPlayer.GetComponent<MP_VR_PlayerController>();
        if (player.Opponent != null)
            m_transTarget = player.Opponent.GetComponentInChildren<HomingTarget>().transform; 
        Invoke("Deactivate", 5.0f);
    }

    public void MakeRandomDirection()
    {
        m_qRandDir = Quaternion.Euler(RandomSign() * Random.Range(m_fMinAngle, m_fMaxAngle), RandomSign() * Random.Range(m_fMinAngle, m_fMaxAngle), 0);
    }

    public int RandomSign()
    {
        int sign = Random.Range(0, 2);
        if (sign == 0)
            sign = -1;

        return sign;
    }

    public void SetRandomDirection(Quaternion q)
    {
        m_qRandDir = q;
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (explosionPrefab != null)
            Instantiate(explosionPrefab, transform.position, transform.rotation);
        GameObject goOther = collision.gameObject;
        if (goOther.layer == LayerMask.NameToLayer("Player"))
        {
            PlayerHit(goOther);
        }
        CancelInvoke();
        Deactivate();
    }

    public override void PlayerHit(GameObject _goPlayer)
    {
        _goPlayer.GetComponentInParent<MP_Health>().TakeDamage(m_iDamage);
    }

    public override void SpellHit()
    {
        throw new System.NotImplementedException();
    }

    // Use this for initialization
    public override void Start ()
    {
		
	}
	
	// Update is called once per frame
	public override void Update ()
    {
		
	}
}
