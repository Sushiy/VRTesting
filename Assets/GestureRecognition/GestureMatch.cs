using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
namespace gesture
{
    public enum gestureTypes
    {
        N_GESTURE,
        Z_GESTURE,
        MIRRORED_N_GESTURE,
        TRIANGLE,
        UNKNOWN
    }

    struct gesture
    {
        public gestureTypes type;
        public Vector2[] points;
    }

    public class GestureMatch : MonoBehaviour
    {
        gesture[] Dataset; // has to be filled now

        // The KNN
        List<KeyValuePair<float, gesture>> FindNearestNeighbors(gesture input, int k)
        {
            var neighbors = new List<KeyValuePair<float, gesture>>();
            foreach(gesture g in Dataset)
            {
                float distance = GestureSimilarity.CompareGestures(input.points, g.points, MeasureType.EUKLIDIAN_MEASURE, false);
                neighbors.Add(new KeyValuePair<float, gesture>(distance, g));
            }
            return neighbors.OrderBy(n => n.Key).Take(k).ToList();
        }

        // Right now: Just prints out a probability table
        void AnalyseNearestNeighbors(ref List<KeyValuePair<float, gesture>> nearestNeighbors)
        {
            int k = nearestNeighbors.Count;
            string[] gestureNames = System.Enum.GetNames(typeof(gestureTypes));
            int gestureCount = gestureNames.Length;
            float[] votes = new float[gestureCount - 1];

            // Get in the votes
            foreach(KeyValuePair<float, gesture> neighbor in nearestNeighbors)
            {
                votes[(int)neighbor.Value.type]++;
            }

            // Make a probability analysis
            for (int i = 0; i < gestureCount; ++i)
            {
                votes[i] /= (float)gestureCount;
                print(gestureNames[i] + " : " + votes[i]);
            }
        }
    }
}
