using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "Vive_", menuName = " Gesture/ViveGesture", order = 1)]
public class ViveGestureData : ScriptableObject
{
    public Vector3[] points;

    public void setPoints(Vector3[] p)
    {
        points = new Vector3[p.Length];
        for (int i=0; i<p.Length; ++i)
        {
            Vector3 vec = new Vector3(p[i].x, p[i].y, p[i].z);
            points[i] = vec;
        }
    }
}
