using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace gesture
{
    /// <summary>
    /// Vordefinierte Gesten, zwischen denen unterschieden werden soll
    /// </summary>
    public enum gestureTypes
    {
        N_GESTURE,
        Z_GESTURE,
        MIRRORED_N_GESTURE,
        TRIANGLE
    }

    /// <summary>
    /// Gesture Data
    /// Ist ein Scriptable Object und enthält die Trainingsdaten, nach denen dann gematched wird
    /// </summary>
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
        public bool initialise = true; // makes it a new blank dataset
        public int NumberOfGestureTypes { get { return System.Enum.GetValues(typeof(gestureTypes)).Length; } }

        /// <summary>
        /// has to be called at least once to be able to fill it
        /// the initialise flag has to be set to be able to fill it again
        /// </summary>
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

        /// <summary>
        /// returns the gestures of a single type
        /// </summary>
        /// <param name="type">returns the gestureDatas of this type</param>
        /// <returns></returns>
        public Vector2[] GetGesturesOfOneType(gestureTypes type)
        {
            return gestures[(int)type];
        }
    }
}