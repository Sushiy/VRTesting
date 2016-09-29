using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;

namespace gesture
{
    public class GestureTester : MonoBehaviour
    {


        private GestureDrawer drawer;
        private GestureMatch matcher;

        private List<KeyValuePair<float, gesture>> testResult;

        void Awake()
        {
            drawer = GameObject.FindGameObjectWithTag("GestureObject").GetComponent<GestureDrawer>();
            matcher = drawer.GetComponent<GestureMatch>();

            // assert refs
            Assert.IsNotNull<GestureDrawer>(drawer);
            Assert.IsNotNull<GestureMatch>(matcher);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.M))
            {
                testResult = null;
                gesture g = new gesture();
                g.points = drawer.getCurrentGesture();
                gestureTypes type;
                bool valid = matcher.Match(g, out type);
                print("This gesture is: " + type.ToString() + " and it is valid: " + valid);
            }
        }
    }

}
