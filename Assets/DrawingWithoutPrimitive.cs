using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using Valve.VR.InteractionSystem;

public class DrawingWithoutPrimitive : MonoBehaviour {

    [SerializeField]
    private float m_fMinimumDistance = 0.1f;

    private Hand hand;
    private LineRenderer line;
    private List<Vector3> points = new List<Vector3>();
    private bool recording = false;
    private Transform endPoint;
    private Vector3 lastPoint;
    

    private void Awake()
    {
        hand = transform.parent.parent.GetComponent<Hand>();
        Assert.IsNotNull(hand);
        line = GetComponent<LineRenderer>();
        Assert.IsNotNull(line);
        endPoint = transform.parent.GetChild(1);
        Assert.IsNotNull(endPoint);
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

            Vector3 newPoint = endPoint.position;

            if (Vector3.Distance(newPoint, lastPoint) > m_fMinimumDistance || line.positionCount == 0)
            {
                points.Add(endPoint.position);
                lastPoint = newPoint;
                line.positionCount++;
                line.SetPositions(points.ToArray());
            }
        }
        else
            recording = false;

	}
}
