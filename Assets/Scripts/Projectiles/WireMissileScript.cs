using UnityEngine;
using System.Collections;

/*This Script enables a missile to be controlled by the player via wand
 */
public class WireMissileScript : Projectiles
{
    public static float m_fMaxRange = 20f;
    public static float m_fBasicSpeed = 30f;
    public static float m_fMaxSteerForce = 200f;
    public static float m_fMaxSteerDistance = 5f;
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
            //m_rigidThis.AddForce(m_transWand.forward * m_fBasicSpeed * Time.fixedDeltaTime, ForceMode.Acceleration);
            float fDistanceToWand = Vector3.Distance(transform.position, m_transWand.position);
            if ( fDistanceToWand <= m_fMaxRange)
            {
                CalculateTargetVector();
                float fActiveMaxSteerDistance = m_fMaxSteerDistance * m_curveSteeringForce.Evaluate(Mathf.Clamp(fDistanceToWand / m_fMaxRange, 0, 1)); ;
                float fMissileSteerDistance = Vector3.Distance(m_v3TargetPosition, m_rigidThis.position);
                float fActiveSteerCoef = m_curveSteeringForce.Evaluate(Mathf.Clamp(fMissileSteerDistance / fActiveMaxSteerDistance, 0.1f, 1));
                WandScript.m_instanceThis.textPanel.text = "Distance: " + fMissileSteerDistance.ToString("F2") + "/" + fActiveMaxSteerDistance.ToString("F2") + "\n"
                                                           + "Force: " + fActiveSteerCoef.ToString("F2") + "/" + 1;
                Vector3 targetDirection = m_v3TargetPosition - m_rigidThis.position;
                m_rigidThis.AddForce( ((targetDirection * fActiveSteerCoef * m_fMaxSteerForce) + (m_transWand.forward * (1.0f- fActiveSteerCoef) * m_fBasicSpeed )) * Time.fixedDeltaTime, ForceMode.Acceleration);
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

        StartCoroutine(ExplodeAfter(5f));
    }

    private void CalculateTargetVector()
    {
        //Wands position is A, wands position at maximum range is B, missile position is P
        Vector3 v3futurePos = m_rigidThis.position;
        Vector3 v3AP = v3futurePos - m_transWand.position;
        Vector3 v3AB = m_transWand.forward * m_fMaxRange;

        float fSqrMagAP = v3AB.sqrMagnitude;

        float fAPdotAB = Vector3.Dot(v3AP,v3AB);

        float t = fAPdotAB / fSqrMagAP;

        m_v3TargetPosition = m_transWand.position + v3AB * t + m_transWand.forward * Time.deltaTime * 5.0f;
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
