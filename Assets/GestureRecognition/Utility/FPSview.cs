using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace gestureUtil
{
    [RequireComponent(typeof(Text))]
    public class FPSview : MonoBehaviour {

        private float deltaTime = 0.0f;
        private Text ui_text;

        void Awake()
        {
            ui_text = GetComponent<Text>();
            Assert.IsNotNull<Text>(ui_text);
        }

        void Update()
        {
            deltaTime += (Time.deltaTime - deltaTime) * 0.1f;

            float msec = deltaTime * 1000.0f;
            float fps = 1.0f / deltaTime;
            ui_text.text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
        }
    }
}
