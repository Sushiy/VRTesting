using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;

namespace gesture
{
    public class GestureTester : MonoBehaviour
    {
        private GestureConverter drawer;
        private GestureMatcher matcher;
        //private LineRenderer line;

        void Awake()
        {
            drawer = GameObject.FindGameObjectWithTag("GestureObject").GetComponent<GestureConverter>();
            matcher = drawer.GetComponent<GestureMatcher>();
            //line = GetComponent<LineRenderer>();

            // assert refs
            Assert.IsNotNull<GestureConverter>(drawer);
            Assert.IsNotNull<GestureMatcher>(matcher);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.M))
            {
                //testResult = null;
                GestureObject g = new GestureObject();
                g.points = drawer.getCurrentGesture(4);
                gestureTypes type;
                bool valid = matcher.Match(g, out type);
                print("This gesture is: " + type.ToString() + " and it is valid: " + valid);
            }

            if (Input.GetKeyDown(KeyCode.N))
            {
                GestureSpellObject g4 = new GestureSpellObject();
                GestureSpellObject g6 = new GestureSpellObject();
                g4.points = drawer.getCurrentGesture(4);
                g6.points = drawer.getCurrentGesture(6);
                SpellType type;
                bool valid = matcher.MultiMatch(new GestureSpellObject[]{g4,g6}, out type);
                print("This gesture is: " + type.ToString() + " and it is valid: " + valid);
            }
        }
    }

}
