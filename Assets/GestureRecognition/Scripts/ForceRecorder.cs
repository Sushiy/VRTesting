using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class ForceRecorder : MonoBehaviour {

    //[SerializeField]
    //private float m_fMaximumVelocity = 100f;
    [SerializeField]
    private float m_fVelocityThreshold = 70f;
    [SerializeField]
    private float m_fTimeBetweenFlicks = 0.1f;
    [SerializeField]
    private MagicWand m_MagicWand;
    public MagicWand MagicWand { get { return m_MagicWand; } }
    [SerializeField]
    private enumFlickDetectingType m_typeFlick = enumFlickDetectingType.STANDARD;
    [SerializeField]
    private float m_fFlickDelay = 0.25f;
    [SerializeField]
    private Vector3 m_v3WandForward = new Vector3(0f, -1f, 0f);
    [SerializeField]
    private float m_fWandVelocityThreshold = 10f;

    private float m_fHighestValue = 0;
    private float m_fTimer = 0f;

    private Rigidbody m_rigid;
    private ParticleSystem m_flickParticles;

    private bool m_bFire = false;
    public Vector3 m_v3velocity;

    private bool m_bWaitingToFlick = false; // for delayed type
    //private Rigidbody m_rigidWand;
    private Vector3 m_lastWandPosition = Vector3.zero;

    private enum enumFlickDetectingType
    {
        STANDARD, DELAYED, POINTER, DELAYED_POINTER, POINTER_NO_WAND_VELO
    }

    void Awake()
    {
        m_rigid = GetComponent<Rigidbody>();
        Assert.IsNotNull(m_MagicWand);

        m_flickParticles = GetComponentInChildren<ParticleSystem>();
        Assert.IsNotNull(m_flickParticles);

        //m_rigidWand = m_MagicWand.GetComponent<Rigidbody>();
        m_lastWandPosition = m_MagicWand.m_SpawnPoint.transform.position;
    }

    public void RemoveFromParent()
    {
        transform.SetParent(null); // unparented
    }

	void FixedUpdate()
    {
        // Flick has been detected and it's waiting for  the
        // wand to halt
        if (m_bWaitingToFlick)
        {
            Vector3 velocity = m_lastWandPosition - m_MagicWand.m_SpawnPoint.transform.position;
            velocity /= Time.deltaTime;
            //if (m_rigidWand.velocity.sqrMagnitude < m_fWandVelocityThreshold)
            if (velocity.sqrMagnitude < m_fWandVelocityThreshold)
                {
                //print("Wand velo: " + velocity.sqrMagnitude);
                FlickDetected();
                m_bWaitingToFlick = false;
            }
        }

        // check if velocity is high enough
        float val = m_rigid.velocity.sqrMagnitude;
        if (val > m_fHighestValue)
        {
            m_fHighestValue = val;
            //print("New Highest Value: " + val);
        }

        // prevent that flicks get detected too often in one motion
        if (m_fTimer > 0f)
        {
            m_fTimer -= Time.deltaTime;
            if (m_fTimer <= 0.005f) m_fTimer = 0f;
        }

        // is the velocity high enough and time has passed?
        if (val > m_fVelocityThreshold && m_fTimer <= 0.005f)
        {
            m_fTimer = m_fTimeBetweenFlicks;

            if (m_typeFlick == enumFlickDetectingType.STANDARD
                || m_typeFlick == enumFlickDetectingType.POINTER)
            {
                FlickDetected();
            }
            else if (m_typeFlick == enumFlickDetectingType.DELAYED
                || m_typeFlick == enumFlickDetectingType.DELAYED_POINTER)
            {
                Invoke("DelayedFlick", m_fFlickDelay);
            }
            else if (m_typeFlick == enumFlickDetectingType.POINTER_NO_WAND_VELO)
            {
                m_bWaitingToFlick = true;
            }
        }

        m_lastWandPosition = m_MagicWand.m_SpawnPoint.transform.position;
    }

    void DelayedFlick()
    {
        FlickDetected();
        m_fTimer = m_fTimeBetweenFlicks;
    }

    // function gets called once a flick is detected
    void FlickDetected()
    {
        // init the particles that get fired when flicked
        if (m_flickParticles.isPlaying)
            m_flickParticles.Stop();
        m_flickParticles.transform.position = transform.position;

        // is the wand loaded? => play the flick particles
        if (m_MagicWand.IsWandLoaded())
            //m_flickParticles.Play();

        //m_MagicWand.FireSpell(m_rigid.velocity);

        // set the velocity
        m_v3velocity = m_rigid.velocity;
        if (m_typeFlick == enumFlickDetectingType.POINTER
            || m_typeFlick == enumFlickDetectingType.DELAYED_POINTER
            || m_typeFlick == enumFlickDetectingType.POINTER_NO_WAND_VELO)
        {
            m_v3velocity = m_MagicWand.transform.localToWorldMatrix * m_v3WandForward;
        }

        // set the fire variable so the networking can fetch the flick-status
        m_bFire = true;
    }

    public bool isFiring()
    {
        if (m_bFire)
        {
            m_bFire = false;
            return true;
        }
        else
            return false;
    }
}
