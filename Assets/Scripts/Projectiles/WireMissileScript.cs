using UnityEngine;
using System.Collections;

/*This Script enables a missile to be controlled by the player via wand
 */
public class WireMissileScript : Projectiles
{
    public float m_fMaximumRange = 20f;
    public float m_fBasicSpeed = 100f;
    public float m_fMaxSteerForce = 20f;
    public float m_fMaxSteerDistance = 20f;
    public AnimationCurve m_curveSteeringForce;

    private Quaternion m_qTargetRotation;
    private Vector3 m_v3TargetPosition;
    private bool m_bWasFired = false;

    public ParticleSystem m_PSExplosion;

    private Rigidbody m_rigidThis;
    public MeshRenderer crystal;
	// Use this for initialization
	void Start ()
    {
        m_rigidThis = GetComponent<Rigidbody>();
        if (m_rigidThis == null)
            Debug.LogError("Rigidbody not found on wiremissile");
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
	    if(m_bWasFired )
        {
            crystal.material.color = Color.blue;
            //This is the basic Force, which is pushing the missile forward at a constant speed (?)
            m_rigidThis.AddForce(transform.forward * m_fBasicSpeed * Time.fixedDeltaTime, ForceMode.Acceleration);
            
            //if(Vector3.Distance(transform.position, m_transWand.position) <= m_fMaximumRange)
            {
                CalculateTargetVector();
                float fMissileSteeringdistance = Vector3.Distance(m_v3TargetPosition, m_rigidThis.position);
                float fActiveSteeringForce = m_fMaxSteerForce * m_curveSteeringForce.Evaluate(fMissileSteeringdistance / m_fMaxSteerDistance);
                Vector3 targetDirection = m_v3TargetPosition - m_rigidThis.position;
                m_rigidThis.AddForce( targetDirection * m_fMaxSteerForce * Time.fixedDeltaTime, ForceMode.Acceleration);
            }

        }
	}

    public override void Fire()
    {
        m_bWasFired = true;
        if (m_rigidThis != null)
            m_rigidThis.AddForce(m_transWand.forward * 3f, ForceMode.Impulse);
        else
        {
            m_rigidThis = GetComponent<Rigidbody>();
            m_rigidThis.AddForce(m_transWand.forward * 3f, ForceMode.Impulse);
        }

        StartCoroutine(ExplodeAfter(3f));
    }

    private void CalculateTargetVector()
    {
        //Wands position is A, wands position at maximum range is B, missile position is P
        Vector3 v3futurePos = m_rigidThis.position + m_rigidThis.velocity * Time.fixedDeltaTime * 2;
        Vector3 v3AP = v3futurePos - m_transWand.position;
        Vector3 v3AB = m_transWand.forward * m_fMaximumRange;

        float fSqrMagAP = v3AB.sqrMagnitude;

        float fAPdotAB = Vector3.Dot(v3AP,v3AB);

        float t = fAPdotAB / fSqrMagAP;

        m_v3TargetPosition = m_transWand.position + v3AB * t;

    }

    public IEnumerator ExplodeAfter(float f)
    {
        yield return new WaitForSeconds(f);
        m_PSExplosion.Play();
        while (m_PSExplosion.IsAlive())
        {
            yield return null;

        }

        Destroy(this.gameObject);
    }
}
