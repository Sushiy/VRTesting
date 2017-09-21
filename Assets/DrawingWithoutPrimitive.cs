using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Valve.VR.InteractionSystem;
using primitive;

namespace gesture
{
    public class DrawingWithoutPrimitive : MonoBehaviour {

        [SerializeField]
        private float m_fMinimumDistance = 0.1f;
        [SerializeField]
        private float m_fTriggerThreshold = 0.1f;
        [SerializeField]
        private float m_fGravityMultiplierParticles = 0.8f;

        private Hand hand;
        private LineRenderer line;
        private List<Vector3> points = new List<Vector3>();
        private bool recording = false;
        private Transform endPoint;
        private Vector3 lastPoint;
        private Transform m_transHelper;

        private GestureConverter m_converter;
        private GestureMatcher m_matcher;
        private MagicWand m_wand;
        private ParticleSystem m_psDrawing;

        private Coroutine m_coroutine_clearLine;
        private Coroutine m_coroutine_particleReaction;

        private bool m_bValidGesture = false;

        private void Awake()
        {
            hand = transform.parent.parent.GetComponent<Hand>();
            Assert.IsNotNull(hand);
            line = GetComponent<LineRenderer>();
            Assert.IsNotNull(line);
            endPoint = transform.parent.GetChild(1);
            Assert.IsNotNull(endPoint);

            m_converter = GameObject.FindGameObjectWithTag("GestureObject").GetComponent<GestureConverter>();
            Assert.IsNotNull(m_converter);
            m_matcher = m_converter.GetComponent<GestureMatcher>();
            Assert.IsNotNull(m_matcher);
            m_wand = transform.parent.GetComponent<MagicWand>();
            Assert.IsNotNull(m_wand);
            m_transHelper = m_converter.transform;
            m_psDrawing = endPoint.GetComponentInChildren<ParticleSystem>();
            Assert.IsNotNull(m_psDrawing);
        }

	    // Update is called once per frame
	    void Update () {
            float trigger = hand.controller.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger).x;

            // when trigger is pushed
            if (trigger > m_fTriggerThreshold)
            {
                // is a line still vanishing? reset!
                if (m_coroutine_clearLine != null)
                {
                    StopCoroutine(m_coroutine_clearLine);
                    m_coroutine_clearLine = null;
                    line.positionCount = 0;
                }

                // are not all the particles gone yet (from previuos drawing)?
                if (m_coroutine_particleReaction != null)
                {
                    StopCoroutine(m_coroutine_particleReaction);
                    m_coroutine_particleReaction = null;
                    if (m_bValidGesture)
                        m_psDrawing.GetComponent<particleAttractorMove>().active = false;
                    else
                    {
                        var main = m_psDrawing.main;
                        main.gravityModifier = 0f;
                    }
                }

                // new recording?
                if (!recording)
                {
                    recording = true;
                    points.Clear();
                    line.positionCount = 0;
                    m_psDrawing.Play();
                }

                AddPoint();
            }
            // one gesture is just finished
            else if (recording)
            {
                recording = false;
                AddPoint();
                PostProcessPoints();
                m_psDrawing.Stop();

                // make the line vanish and suck in the particles
                m_coroutine_clearLine = StartCoroutine(coroutine_clearLine());
                m_coroutine_particleReaction = StartCoroutine(coroutine_particleReaction(m_bValidGesture));
            }

	    }

        void AddPoint()
        {
            Vector3 newPoint = endPoint.position;

            if (Vector3.Distance(newPoint, lastPoint) > m_fMinimumDistance || line.positionCount == 0)
            {
                points.Add(endPoint.position);
                lastPoint = newPoint;
                line.positionCount++;
                line.SetPositions(points.ToArray());
            }
        }

        // post process the drawn thing to make a gesture and match
        void PostProcessPoints()
        {
            // store points in array
            Vector3[] p = points.ToArray();

            // calculate center;
            Vector3 center, normal, min, max;
            m_converter.IdentifyMinMax(ref p, out min, out max);
            center.x = min.x + 0.5f * (max.x - min.x);
            center.y = min.y + 0.5f * (max.y - min.y);
            center.z = min.z + 0.5f * (max.z - min.z);

            // reposition gesture through rotation to align on z axis
            for (int i = 0; i < p.Length; ++i) p[i] = p[i] - Camera.main.transform.position;
            m_transHelper.LookAt(center - Camera.main.transform.position);
            Quaternion q = m_transHelper.transform.localRotation;
            Quaternion qi = Quaternion.Inverse(q);
            for (int i = 0; i < p.Length; ++i) p[i] = qi * p[i];

            /* set the normal */
            normal = new Vector3(0f, 0f, 1f);

            GestureObject g = m_converter.CreateGestureFrom3DData(ref p, normal);

            // match the gesture
            gestureTypes type;
            bool valid = m_matcher.Match(g, out type);
            print("Gesture is vald=" + valid.ToString() + " and " + type.ToString());

            // charge wand if valid
            if (valid)
            {
                m_wand.LoadWand(DrawingOnPrimitive.m_gestureLUT[(int)type]);
            }

            m_bValidGesture = valid;
        }

        // is called when the gesture is done
        IEnumerator coroutine_clearLine()
        {
            while (points.Count > 0)
            {
                AddPoint(); // stay connected to the wand
                points.RemoveAt(0);
                line.positionCount = points.Count;
                line.SetPositions(points.ToArray());
                yield return null;
            }
        }

        // Is Called after a finished gesture
        IEnumerator coroutine_particleReaction(bool validGesture)
        {
            // if its a valid gesture, suck in the particles
            if (validGesture)
            {
                m_psDrawing.GetComponent<particleAttractorMove>().active = true;
                while (m_psDrawing.IsAlive())
                    yield return null;
                m_psDrawing.GetComponent<particleAttractorMove>().active = false;
            }
            // else, just drop the particles on the floor
            else
            {
                var main = m_psDrawing.main;
                main.gravityModifier = m_fGravityMultiplierParticles;
                while (m_psDrawing.IsAlive())
                    yield return null;
                main.gravityModifier = 0f;
            }
        }
    }
}
