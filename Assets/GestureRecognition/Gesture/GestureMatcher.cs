using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
namespace gesture
{
    public struct GestureObject
    {
        public gestureTypes type;
        public Vector2[] points;
    }

    /// <summary>
    /// GestureMatch
    /// 
    /// Takes a given gesture dataset and a gesture and tries to match the gesture
    /// against the dataset using the kNN (k-nearest-neighbor) algorithm.
    /// </summary>
    public class GestureMatcher : MonoBehaviour
    {
        [SerializeField]
        private GestureData dataObject;
        [Range(0f, 3f)]
        [SerializeField]
        private float avgDistanceThreshold = 1f;
        [Range(0, 15)]
        [Tooltip("Should be an odd number!")]
        [SerializeField]
        private int m_k = 3;
        [SerializeField]
        private TextMesh debug_text; //debug

        private GestureObject[] Dataset; // has to be filled now

        /// <summary>
        /// takes the data from the GestureData-dataObject and converts it into
        /// a gesture-Array "Dataset" so its easier to access for the kNN algorithm
        /// </summary>
        void Awake()
        {
            if (dataObject == null)
                Debug.LogError("No dataset put in to match to!");
            else
                dataObject.Init();

            Dataset = new GestureObject[dataObject.NumberOfGestureTypes * dataObject.samplesPerGesture];

            // convert GestureData to a gesture array
            // go through every gesture type
            for (int typeIndex = 0; typeIndex < dataObject.NumberOfGestureTypes; ++typeIndex)
            {
                // iterate over the number of samples per type
                for (int sampleIndex = 0; sampleIndex < dataObject.samplesPerGesture; ++sampleIndex)
                {
                    GestureObject g = new GestureObject();
                    g.type = (gestureTypes)typeIndex;
                    g.points = new Vector2[dataObject.pointsPerGesture];

                    // and copy every single point to a new gesture
                    for (int pointIndex = 0; pointIndex < dataObject.pointsPerGesture; ++pointIndex)
                    {
                        g.points[pointIndex] = dataObject.gestures[typeIndex][sampleIndex * dataObject.pointsPerGesture + pointIndex];
                    }

                    // then add that gesture
                    Dataset[typeIndex * dataObject.samplesPerGesture + sampleIndex] = g;
                }
            }
        }

        /// <summary>
        /// The kNN algorithm.
        /// </summary>
        /// <param name="input">The gesture to find the k-nearest neighbors to</param>
        /// <param name="k">The number of neighbors to find (should be odd)</param>
        /// <returns>List of k-many (distance/gesture) tuples</returns>
        private List<KeyValuePair<float, GestureObject>> FindNearestNeighbors(GestureObject input, int k)
        {
            // check for traps
            if (input.points.Length != dataObject.pointsPerGesture)
            {
                Debug.LogError("The gestures to compare have different counts of points! Make sure both gestures have the same number of points."+
                    "Input length: " + input.points.Length + "; needed nr of points: "+dataObject.pointsPerGesture);
                return null;
            }

            var neighbors = new List<KeyValuePair<float, GestureObject>>();
            foreach(GestureObject g in Dataset)
            {
                float distance = GestureSimilarity.CompareGestures(input.points, g.points, MeasureType.EUKLIDIAN_MEASURE, false);
                if (distance == -1)
                {
                    print("The gestures to compare have different counts of points! Make sure both gestures have the same number of points");
                }
                neighbors.Add(new KeyValuePair<float, GestureObject>(distance, g));
            }
            return neighbors.OrderBy(n => n.Key).Take(k).ToList();
        }

        /// <summary>
        /// Prints out a probability table of the given gesture.
        /// </summary>
        /// <param name="input"></param>
        public void AnalyseNearestNeighbors(GestureObject input)
        {
            var nearestNeighbors = FindNearestNeighbors(input, m_k);

            string[] gestureNames = System.Enum.GetNames(typeof(gestureTypes));
            int gestureCount = gestureNames.Length;
            float[] votes = new float[gestureCount];
            float[] avgDistances = new float[gestureCount];

            // Get in the votes
            foreach(KeyValuePair<float, GestureObject> neighbor in nearestNeighbors)
            {
                votes[(int)neighbor.Value.type]++;
                avgDistances[(int)neighbor.Value.type] += neighbor.Key;
            }

            // Make a probability analysis
            for (int i = 0; i < gestureCount; ++i)
            {
                avgDistances[i] /= votes[i];
                votes[i] /= nearestNeighbors.Count;
                print(gestureNames[i] + " : " + (votes[i]*100f) + "%; Avg Distance: "+ avgDistances[i]);
            }
        }

        /// <summary>
        /// This method matches the given input gesture against the dataset.
        /// Using the kNN algorithm it always finds a match.
        /// The match gets validated by the avgDistance via the threshold.
        /// </summary>
        /// <param name="input">The input gesture</param>
        /// <param name="matchedType">out goes the matched type, the result of the match whether its valid or not</param>
        /// <returns>Returns true if its valid (against the threshold), otherwise false</returns>
        public bool Match(GestureObject input, out gestureTypes matchedType)
        {
            var nearestNeighbors = FindNearestNeighbors(input, m_k);
            if (nearestNeighbors == null)
            {
                matchedType = gestureTypes.N_GESTURE;
                return false;
            }

            int gestureCount = dataObject.NumberOfGestureTypes;

            Vector2[] votes = new Vector2[dataObject.NumberOfGestureTypes];

            // Get in the votes
            foreach (KeyValuePair<float, GestureObject> neighbor in nearestNeighbors)
            {
                votes[(int)neighbor.Value.type].x++;
                votes[(int)neighbor.Value.type].y += neighbor.Key;
            }

            // Get the best match
            int matchIndex = -1;
            float highestMatchValue = float.MinValue;
            for (int i = 0; i < gestureCount; ++i)
            {
                votes[i].y /= votes[i].x;
                votes[i].x /= nearestNeighbors.Count;

                if (votes[i].x > highestMatchValue)
                {
                    matchIndex = i;
                    highestMatchValue = votes[i].x;
                }
            }
            
            // set the matched type
            matchedType = (gestureTypes)matchIndex;
            // is the average distance low enough?
            bool valid = (votes[matchIndex].y <= avgDistanceThreshold) ? true : false;

            // write out debug text
            if (debug_text != null) debug_text.text = "This gesture is: " + matchedType.ToString() + " and it is valid: " + valid;

            return valid;
        }
    }
}
