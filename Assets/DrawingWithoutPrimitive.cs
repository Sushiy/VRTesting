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

        private Hand hand;
        private LineRenderer line;
        private List<Vector3> points = new List<Vector3>();
        private bool recording = false;
        private Transform endPoint;
        private Vector3 lastPoint;
        private LineRenderer debugLine1;
        private LineRenderer debugLine2;

        private GestureConverter m_converter;
        private GestureMatcher m_matcher;
        private MagicWand m_wand;



        private void Awake()
        {
            hand = transform.parent.parent.GetComponent<Hand>();
            Assert.IsNotNull(hand);
            line = GetComponent<LineRenderer>();
            Assert.IsNotNull(line);
            endPoint = transform.parent.GetChild(1);
            Assert.IsNotNull(endPoint);
            debugLine1 = transform.GetChild(0).GetComponent<LineRenderer>();
            Assert.IsNotNull(debugLine1);
            debugLine2 = transform.GetChild(1).GetComponent<LineRenderer>();
            Assert.IsNotNull(debugLine2);

            m_converter = GameObject.FindGameObjectWithTag("GestureObject").GetComponent<GestureConverter>();
            Assert.IsNotNull(m_converter);
            m_matcher = m_converter.GetComponent<GestureMatcher>();
            Assert.IsNotNull(m_matcher);
            m_wand = transform.parent.GetComponent<MagicWand>();
            Assert.IsNotNull(m_wand);
        }

	    // Update is called once per frame
	    void Update () {
            float trigger = hand.controller.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger).x;

            // when trigger is pushed
            if (trigger > 0f)
            {
                // new recording?
                if (!recording)
                {
                    recording = true;
                    points.Clear();
                    line.positionCount = 0;
                }

                AddPoint();
            }
            else if (recording)
            {
                recording = false;
                AddPoint();
                PostProcessPoints();
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

        void PostProcessPoints()
        {
            // post process the drawn thing
            Vector3[] p = points.ToArray();
            Vector3 normal = Camera.main.transform.forward;
            Vector2[] gesturePoints;

            GestureConverter.Transform3DData(ref p, normal, out gesturePoints);
            for (int i=0; i<p.Length; ++i)
            {
                p[i] = new Vector3(gesturePoints[i].x, gesturePoints[i].y, 0f);
            }
            debugLine1.positionCount = p.Length;
            debugLine1.SetPositions(p); // drawing the xy-plane-projected points to test the transform3ddata call

            // create gesture (debug, seems to not work yet, meh)
            GestureObject g = m_converter.CreateGestureFrom3DData(ref p, normal);
            string s = "";
            for (int i = 0; i<g.points.Length; ++i)
            {
                s += g.points[i].ToString() + "\n";
            }
            print(s);
            debugLine2.positionCount = g.points.Length;
            debugLine2.SetPosition(0, new Vector3(g.points[0].x, g.points[0].y, 0f));
            debugLine2.SetPosition(1, new Vector3(g.points[1].x, g.points[1].y, 0f));
            debugLine2.SetPosition(2, new Vector3(g.points[2].x, g.points[2].y, 0f));
            debugLine2.SetPosition(3, new Vector3(g.points[3].x, g.points[3].y, 0f));

            // match the gesture
            gestureTypes type;
            bool valid = m_matcher.Match(g, out type);
            print("Gesture is vald=" + valid.ToString() + " and " + type.ToString());

            // charge wand if valid
            if (valid)
            {
                m_wand.LoadWand(DrawingOnPrimitive.m_gestureLUT[(int)type]);
            }
        }
    }
}
