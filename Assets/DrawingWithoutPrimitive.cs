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
        //private LineRenderer debugLine1;
        //private LineRenderer debugLine2;
        //private LineRenderer debugLine3;
        private Transform m_transHelper;

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

            // debug lines
            //debugLine1 = transform.GetChild(0).GetComponent<LineRenderer>();
            //Assert.IsNotNull(debugLine1);
            //debugLine2 = transform.GetChild(1).GetComponent<LineRenderer>();
            //debugLine3 = transform.GetChild(2).GetComponent<LineRenderer>();
            //Assert.IsNotNull(debugLine2);
            //Assert.IsNotNull(debugLine3);

            m_converter = GameObject.FindGameObjectWithTag("GestureObject").GetComponent<GestureConverter>();
            Assert.IsNotNull(m_converter);
            m_matcher = m_converter.GetComponent<GestureMatcher>();
            Assert.IsNotNull(m_matcher);
            m_wand = transform.parent.GetComponent<MagicWand>();
            Assert.IsNotNull(m_wand);
            m_transHelper = m_converter.transform;
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
            //debugLine2.positionCount = p.Length;
            //debugLine2.SetPositions(p);
            m_transHelper.LookAt(center - Camera.main.transform.position);
            Quaternion q = m_transHelper.transform.localRotation;
            Quaternion qi = Quaternion.Inverse(q);
            for (int i = 0; i < p.Length; ++i) p[i] = qi * p[i];
            //debugLine3.positionCount = p.Length;
            //debugLine3.SetPositions(p);

            /* set the normal */
            normal = new Vector3(0f, 0f, 1f);

            GestureObject g = m_converter.CreateGestureFrom3DData(ref p, normal);

            //// draw the gesture points
            //debugLine1.positionCount = 0;
            //for (int i = 0; i < g.points.Length; ++i)
            //{
            //    debugLine1.positionCount++;
            //    debugLine1.SetPosition(i, new Vector3(g.points[i].x, g.points[i].y + 1, 0f));
            //}

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
