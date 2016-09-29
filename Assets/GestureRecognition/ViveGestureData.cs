using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "Vive_", menuName = " Gesture/ViveGesture", order = 1)]
public class ViveGestureData : ScriptableObject
{
    public Vector3[] points;
}
