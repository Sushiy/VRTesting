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
        FIRE_GESTURE_1,
        FIRE_GESTURE_2,
        METEOR_GESTURE_1,
        METEOR_GESTURE_2,
        SHIELD_GESTURE_1,
        SHIELD_GESTURE_2,
        MISSILE_GESTURE_1,
        MISSILE_GESTURE_2,
        BEAM_GESTURE_1,
        BEAM_GESTURE_2
    }

    /// <summary>
    /// Gesture Data
    /// Ist ein Scriptable Object und enthält die Trainingsdaten, nach denen dann gematched wird
    /// </summary>
    [CreateAssetMenu(fileName = "GestureDataset_", menuName = "Gesture/Dataset", order = 1)]
    public class GestureData : ScriptableObject
    {
        public Vector2[][] gestures;

        public Vector2[] fire_gesture_1;
        public Vector2[] fire_gesture_2;
        public Vector2[] meteor_gesture_1;
        public Vector2[] meteor_gesture_2;
        public Vector2[] shield_gesture_1;
        public Vector2[] shield_gesture_2;
        public Vector2[] missile_gesture_1;
        public Vector2[] missile_gesture_2;
        public Vector2[] beam_gesture_1;
        public Vector2[] beam_gesture_2;

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

            gestures[0] = fire_gesture_1;
            gestures[1] = fire_gesture_2;
            gestures[2] = meteor_gesture_1;
            gestures[3] = meteor_gesture_2;
            gestures[4] = shield_gesture_1;
            gestures[5] = shield_gesture_2;
            gestures[6] = missile_gesture_1;
            gestures[7] = missile_gesture_2;
            gestures[8] = beam_gesture_1;
            gestures[9] = beam_gesture_2;

            if (!initialise)
                return;

            fire_gesture_1 = new Vector2[samplesPerGesture * pointsPerGesture];
            fire_gesture_2 = new Vector2[samplesPerGesture * pointsPerGesture];
            meteor_gesture_1 = new Vector2[samplesPerGesture * pointsPerGesture];
            meteor_gesture_2 = new Vector2[samplesPerGesture * pointsPerGesture];
            shield_gesture_1 = new Vector2[samplesPerGesture * pointsPerGesture];
            shield_gesture_2 = new Vector2[samplesPerGesture * pointsPerGesture];
            missile_gesture_1 = new Vector2[samplesPerGesture * pointsPerGesture];
            missile_gesture_2 = new Vector2[samplesPerGesture * pointsPerGesture];
            beam_gesture_1 = new Vector2[samplesPerGesture * pointsPerGesture];
            beam_gesture_2 = new Vector2[samplesPerGesture * pointsPerGesture];

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