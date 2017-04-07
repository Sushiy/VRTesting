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

        //private List<KeyValuePair<float, gesture>> testResult;

        void Awake()
        {
            drawer = GameObject.FindGameObjectWithTag("GestureObject").GetComponent<GestureConverter>();
            matcher = drawer.GetComponent<GestureMatcher>();

            // assert refs
            Assert.IsNotNull<GestureConverter>(drawer);
            Assert.IsNotNull<GestureMatcher>(matcher);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.M))
            {
                //testResult = null;
                gesture g = new gesture();
                g.points = drawer.getCurrentGesture();
                gestureTypes type;
                bool valid = matcher.Match(g, out type);
                print("This gesture is: " + type.ToString() + " and it is valid: " + valid);
            }
        }
    }

}
