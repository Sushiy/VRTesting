using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace gesture
{
    public enum gestureTypes
    {
        N_GESTURE,
        Z_GESTURE,
        MIRRORED_N_GESTURE,
        TRIANGLE
    }

    [CreateAssetMenu(fileName = "GestureDataset_", menuName = " Gesture/Dataset", order = 1)]
    public class GestureData : ScriptableObject
    {
        public Vector2[][] gestures;

        public Vector2[] n_gesture;
        public Vector2[] z_gesture;
        public Vector2[] mirrored_n_gesture;
        public Vector2[] triangle;
        
        public int samplesPerGesture = 4; // how many samples of one gesture?
        public int pointsPerGesture = 4; // points per gesture
        public bool initialise = true;
        public int NumberOfGestureTypes { get { return System.Enum.GetValues(typeof(gestureTypes)).Length; } }

        //  has to be called at least once
        public void Init()
        {
            gestures = new Vector2[NumberOfGestureTypes][];

            gestures[0] = n_gesture;
            gestures[1] = z_gesture;
            gestures[2] = mirrored_n_gesture;
            gestures[3] = triangle;

            if (!initialise)
                return;

            n_gesture = new Vector2[samplesPerGesture * pointsPerGesture];
            z_gesture = new Vector2[samplesPerGesture * pointsPerGesture];
            mirrored_n_gesture = new Vector2[samplesPerGesture * pointsPerGesture];
            triangle = new Vector2[samplesPerGesture * pointsPerGesture];

            initialise = false;
        }

        // returns the gestures of a single type
        public Vector2[] GetGesturesOfOneType(gestureTypes type)
        {
            return gestures[(int)type];
        }

        // prints out, how many gestures have been recorded for each gesture
        //public string PrintData()
        //{
        //    string[] names = System.Enum.GetNames(typeof(gestureTypes));
        //    string result = "";
        //    for (int i=0; i<names.Length; ++i)
        //    {
        //        int zeroCount = 0;
        //        for (int j=0; j<pointsPerGesture; ++j)
        //        {
        //            if (gestures[i][j] == Vector2.zero) zeroCount++;
        //        }
        //        int gesturesRecorded = ((samplesPerGesture * pointsPerGesture) - zeroCount) / 4;

        //        result += "Number of " + names[i] + ": " + gesturesRecorded;
        //    }

        //    return result;
        //}
    }
}