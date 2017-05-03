using UnityEngine;
using System.Collections;

namespace gesture
{
    public enum MeasureType
    {
        EUKLIDIAN_MEASURE,
        MANHATTEN_MEASURE   
    }

    /// <summary>
    /// GestureSimilarity
    /// A class that basically measures the similarity by either
    /// euklidian or manhatten measure.
    /// </summary>
    public class GestureSimilarity
    {
        /// <summary>
        /// Compares Gestures
        /// </summary>
        /// <param name="g1">The first gesture</param>
        /// <param name="g2">The second gesture</param>
        /// <param name="type">The type of measurement</param>
        /// <param name="rootIt">Whether or not the result should get square-rooted (performance)</param>
        /// <returns></returns>
        public static float CompareGestures(Vector2[] g1, Vector2[] g2, MeasureType type, bool rootIt)
        {
            // Check for traps
            int n = g1.Length;
            if (n != g2.Length)
            {
                return -1; // ERROR
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

        /// <summary>
        /// returns the Euklidian Measure of two Vectors
        /// </summary>
        /// <param name="a">First Vector</param>
        /// <param name="b">Second Vector</param>
        /// <returns>Euklidian Distance of the vectors</returns>
        static float EuklidianMeasure(Vector2 a, Vector2 b)
        {
            return Vector2.Distance(a, b);
        }

        /// <summary>
        /// returns the ManhattenMeasure of two Vectors
        /// </summary>
        /// <param name="a">First Vector</param>
        /// <param name="b">Second Vector</param>
        /// <returns>Manhatten Distance of the vectors</returns>
        static float ManhattenMeasure(Vector2 a, Vector2 b)
        {
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
        }
    }
}