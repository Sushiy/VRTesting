using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using gesture;

public class GestureTutorial : MonoBehaviour {

    public GestureDataFlat m_dataset;
    public float m_fLengthMultiplier = 1f;
    public int m_iStepSize = 5;
    public int[] m_iArrSampleIndices;
    public float m_fGestureWait = 1f;

    public delegate void NextStepAction();
    public static event NextStepAction OnShowNextGesture;
    public static event NextStepAction OnStartFlickAnimation;

    private int m_iGestureIndex = 0;
    private Coroutine m_coroutine_showGesture;
    private Vector3 m_v3OriginalPosition;
    private Quaternion m_qOriginalRotation;
    private Transform m_transThis;
    private TrailRenderer m_trail;
    private Animator m_animator;
    private ParticleSystem m_psPoof;

    private void Awake()
    {
        m_transThis = transform;
        m_v3OriginalPosition = m_transThis.position;
        m_trail = m_transThis.GetComponentInChildren<TrailRenderer>();
        m_animator = GetComponent<Animator>();
        m_qOriginalRotation = m_transThis.rotation;
        m_psPoof = GetComponentInChildren<ParticleSystem>();
    }

    private void OnEnable()
    {
        OnShowNextGesture += ShowNextGesture;
        OnStartFlickAnimation += StartFlickAnimation;
    }

    private void OnDisable()
    {
        OnShowNextGesture -= ShowNextGesture;
        OnStartFlickAnimation -= StartFlickAnimation;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O)) OnShowNextGesture();
        if (Input.GetKeyDown(KeyCode.P)) OnStartFlickAnimation();
    }

    private void StartFlickAnimation()
    {
        m_trail.Clear();
        m_trail.enabled = false;
        m_psPoof.Stop();
        m_psPoof.Play();
        if (m_coroutine_showGesture != null)
        {
            StopCoroutine(m_coroutine_showGesture);
            m_coroutine_showGesture = null;
        }
        m_transThis.position = m_v3OriginalPosition;
        m_transThis.rotation = m_qOriginalRotation;
        m_animator.SetBool("Flick", true);
    }

    private void ShowNextGesture()
    {
        m_psPoof.Stop();
        m_trail.enabled = true;
        m_animator.SetBool("Flick", false);
        m_transThis.rotation = m_qOriginalRotation;

        if (m_coroutine_showGesture != null)
        {
            StopCoroutine(m_coroutine_showGesture);
            m_coroutine_showGesture = null;
        }

        m_coroutine_showGesture = StartCoroutine(coroutine_showGesture());
    }

    IEnumerator coroutine_showGesture()
    {
        // fetch the datapoints of the next gesture
        int sampleIndex = 0;
        if (m_iArrSampleIndices.Length > m_iGestureIndex)
            sampleIndex = m_iArrSampleIndices[m_iGestureIndex]; 

        Vector2[] points2D = m_dataset.getPointsFromSample(m_iGestureIndex++, sampleIndex); // automatically iterate over the gestureIndex
        
        // start the animation of the gesture
        while (true)
        {
            // start from  the beginning!
            m_transThis.position = m_v3OriginalPosition;
            // clear the trail
            m_trail.Clear();
            // for every segent
            for (int i=0; i<points2D.Length-1; ++i)
            {
                // calculate the positions for this segment
                float t = 0f;
                Vector3 oldPos = m_transThis.position;
                Vector2 v = (points2D[i + 1] - points2D[i]) * m_fLengthMultiplier;
                Vector3 newPos = m_transThis.position + new Vector3(0f, v.y, -v.x);
                // and move the dagger
                while (t < 1f)
                {
                    t += 1f / (2 << m_iStepSize);
                    m_transThis.position = Vector3.Lerp(oldPos, newPos, t);
                    yield return null;
                }
            }
            yield return new WaitForSeconds(m_fGestureWait);
        }
    }
}
