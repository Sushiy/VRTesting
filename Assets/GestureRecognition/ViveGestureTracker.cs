using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ViveGestureTracker : MonoBehaviour {

    [SerializeField]
    private ViveGestureData dataset;
    [SerializeField]
    private SteamVR_TrackedObject controller;
    private int m_iDeviceIndexThis = -1;

    private List<Vector3> points = new List<Vector3>();
    private bool tracking = false;
    private Transform _transform;

    void Awake()
    {
        _transform = transform;
    }

    void Update()
    {
        if (m_iDeviceIndexThis == -1)
            m_iDeviceIndexThis = (int)controller.index;

        if (Input())
        {
            tracking = true;
            points.Add(_transform.position);
        }
        else if (tracking)
        {
            dataset.points = points.ToArray();
            points.Clear();
            tracking = false;
        }
    }

    bool Input()
    {
        var device = SteamVR_Controller.Input(m_iDeviceIndexThis);
        return device.GetPress(SteamVR_Controller.ButtonMask.Trigger);
    }
}
