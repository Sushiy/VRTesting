using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using gesture;

public class GestureTutorial : MonoBehaviour {

    [SerializeField] private GestureDataFlat[] m_datasets;
    [SerializeField] private float m_fLengthMultiplier = 1f;
    [SerializeField] private int m_iStepSize = 5;
    [SerializeField] private float m_fGestureWait = 1f;
    [SerializeField] private SpellType[] spellTypes;
    [SerializeField] private int[] m_iArrSampleIndices;

    private GestureSpellObject[] gestureObjects;
    private int m_iGestureIndex = 0;
    private Coroutine m_coroutine_showGesture;
    private Vector3 m_v3OriginalPosition;
    private Quaternion m_qOriginalRotation;
    private Transform m_transThis;
    private TrailRenderer m_trail;
    private Animator m_animator;
    private ParticleSystem m_psPoof;
    private bool m_bFlicking = false;

    public static GestureTutorial s_instance; // singleton!

    private void Awake()
    {
        if (s_instance != null) Destroy(s_instance);
        s_instance = this;

        m_transThis = transform;
        m_v3OriginalPosition = m_transThis.position;
        m_trail = m_transThis.GetComponentInChildren<TrailRenderer>();
        m_animator = GetComponent<Animator>();
        m_qOriginalRotation = m_transThis.rotation;
        m_psPoof = GetComponentInChildren<ParticleSystem>();

        InitGestures();

        gameObject.SetActive(false);
    }

    // "Autoplay"
    public void InvokeStart(float time)
    {
        Invoke("ShowCurrentGesture", time);
    }

    private void InitGestures()
    {
        gestureObjects = new GestureSpellObject[spellTypes.Length];
        var allGestures = new List<GestureSpellObject>();
        foreach (GestureDataFlat dataset in m_datasets)
        {
            GestureSpellObject[] gestures;
            dataset.createGestureDataset(out gestures);
            allGestures.AddRange(gestures);
        }

        foreach(GestureSpellObject go in allGestures)
        {
            // check what ID the spelltype has
            int spelltypeID = -1; // the index for the spelltypes and sampleindices array
            for (int i = 0; i < spellTypes.Length; ++i) if (spellTypes[i] == go.type) { spelltypeID = i; break; }
            if (spelltypeID < 0) continue; // if this spelltype isn't needed, skip this

            // is it not searched anymore?
            if (m_iArrSampleIndices[spelltypeID] < 0) continue; // then skip

            gestureObjects[spelltypeID] = go;
            m_iArrSampleIndices[spelltypeID]--;
        }
    }

    public void WandLoaded(SpellType type)
    {
        if (type == spellTypes[m_iGestureIndex])
        {
            StartFlickAnimation();
        }
        else
        {
            ShowCurrentGesture();
        }
    }

    public void WandFired()
    {
        if (m_bFlicking)
        {
            m_iGestureIndex++;
            
            if (m_iGestureIndex >= spellTypes.Length)
            {
                StopAllCoroutines();
                Destroy(gameObject);
            }

            ShowCurrentGesture();
        }
    }

    private void Update()
    {
    }

    private void StartFlickAnimation()
    {
        if (m_coroutine_showGesture != null)
        {
            StopCoroutine(m_coroutine_showGesture);
            m_coroutine_showGesture = null;
        }
        m_trail.Clear();
        m_trail.enabled = false;
        m_psPoof.Stop();
        m_psPoof.Play();
        m_transThis.position = m_v3OriginalPosition;
        m_transThis.rotation = m_qOriginalRotation;
        m_animator.SetBool("Flick", true);
        m_bFlicking = true;
    }

    private void ShowCurrentGesture()
    {
        if (m_coroutine_showGesture != null)
        {
            return;
        }

        m_bFlicking = false;
        m_psPoof.Stop();
        m_animator.SetBool("Flick", false);
        m_transThis.position = m_v3OriginalPosition;
        m_transThis.rotation = m_qOriginalRotation;

        m_coroutine_showGesture = StartCoroutine(coroutine_showGesture());
    }

    IEnumerator coroutine_showGesture()
    {
        bool skip = m_iGestureIndex >= gestureObjects.Length; // if gestureindex is too high, just skip everything!

        // start the animation of the gesture
        while (!skip)
        {
            // fetch the points
            Vector2[] points2D = gestureObjects[m_iGestureIndex].points;
            // start from  the beginning!
            m_trail.enabled = true;
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
                    t += (1f / (2 << m_iStepSize)) * Time.deltaTime * 60f;
                    m_transThis.position = Vector3.Lerp(oldPos, newPos, t);
                    yield return null;
                }
            }
            yield return new WaitForSeconds(m_fGestureWait);
        }
    }
}
