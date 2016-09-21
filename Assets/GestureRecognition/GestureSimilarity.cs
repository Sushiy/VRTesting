using UnityEngine;
using System.Collections;

namespace gesture
{
    enum MeasureType
    {
        EUKLIDIAN_MEASURE,
        MANHATTEN_MEASURE   
    }

    public class GestureSimilarity : MonoBehaviour
    {
        float CompareGestures(Vector2[] g1, Vector2[] g2, MeasureType type, bool rootIt)
        {
            // Check for traps
            int n = g1.Length;
            if (n != g2.Length)
            {
                print("The gestures to compare have different counts of points! Make sure both gestures have the same number of points");
                return -1;
            }

            // Compare each
            float result = 0;
            for (int i=0; i<n; ++i)
            {
                switch (type)
                {
                    case MeasureType.EUKLIDIAN_MEASURE: result += EuklidianMeasure(g1[i], g2[i]); break;
                    case MeasureType.MANHATTEN_MEASURE: result += ManhattenMeasure(g1[i], g2[i]); break;
                }
            }

            // Take the Square Root?
            if (rootIt)
                result = Mathf.Sqrt(result);

            return result;
        }

        // returns the Euklidian Measure of two Vectors
        float EuklidianMeasure(Vector2 a, Vector2 b)
        {
            return Vector2.Distance(a, b);
        }

        // returns the ManhattenMeasure of two Vectors
        float ManhattenMeasure(Vector2 a, Vector2 b)
        {
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
        }
    }
}