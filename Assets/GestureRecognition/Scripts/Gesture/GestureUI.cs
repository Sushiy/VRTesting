﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Collections;

namespace gesture
{
    /// <summary>
    /// Creates and manages UI elements to record gestures to fill a
    /// gesture-daraset
    /// </summary>
    public class GestureUI : MonoBehaviour
    {
        [SerializeField]
        private GameObject buttonPrefab;
        [SerializeField]
        private Color indexColor;
        [SerializeField]
        private Color gestureColor;
        [SerializeField]
        private Color indexActiveColor;
        [SerializeField]
        private Color indexFullColor;
        [SerializeField]
        private Color gestureActiveColor;

        private Transform indexPanel;
        private Transform typePanel;
        private GestureDataRecorder recorder;
        private GestureData dataset;

        private Image[] gesturePanels;
        private Image[] indexPanels;
        private int[] gesturesRecorded; // used as a mask

        public int selectedGesture { private set; get; }
        public int selectedIndex { private set; get; }

        char[] gestureCharPool = { 'q', 'w', 'e', 'r', 't', 'z', 'u', 'i' };
        char[] indexCharPool = { '1', '2', '3', '4', '5', '6', '7', '8', '9', '0' };

        void Awake()
        {
            selectedGesture = -1;
            selectedIndex = -1;

            indexPanel = transform.GetChild(0);
            typePanel = transform.GetChild(1);
            recorder = GameObject.FindGameObjectWithTag("GestureObject").GetComponent<GestureDataRecorder>();
            dataset = recorder.Dataset;

            // assert
            Assert.IsNotNull<Transform>(indexPanel);
            Assert.IsNotNull<Transform>(typePanel);
            Assert.IsNotNull<GestureDataRecorder>(recorder);
            Assert.IsNotNull<GestureData>(dataset);
        }

        void Start()
        {
            gesturesRecorded = new int[dataset.pointsPerGesture];

            indexPanels = new Image[dataset.samplesPerGesture];
            gesturePanels = new Image[dataset.pointsPerGesture];

            // create indexpanel
            for (int i=0; i<dataset.samplesPerGesture; ++i)
            {
                Transform b = Instantiate(buttonPrefab).transform;
                b.name = "index_" + i;
                b.SetParent(indexPanel);
                b.GetChild(0).GetComponent<Text>().text = "Index " + i + "["+indexCharPool[i]+"]";
                b.GetComponent<Image>().color = indexColor;
                indexPanels[i] = b.GetComponent<Image>();
            }

            // create gesture panel buttons
            string[] names = System.Enum.GetNames(typeof(gestureTypes));
            for (int i = 0; i < dataset.pointsPerGesture; ++i)
            {
                Transform b = Instantiate(buttonPrefab).transform;
                b.name = "type_" + i;
                b.SetParent(typePanel);
                b.GetChild(0).GetComponent<Text>().text = names[i] + "[" + gestureCharPool[i] + "]";
                b.GetComponent<Image>().color = gestureColor;
                gesturePanels[i] = b.GetComponent<Image>();
            }

            SelectGesture(0);
        }

        void Update()
        {
            char c;
            char.TryParse(Input.inputString, out c);

            // index
            if (char.IsDigit(c))
            {
                int index = FindChar(c, ref indexCharPool);
                if (index == -1)
                    return;
                SelectIndex(index);
            }
            // gesture
            else if (char.IsLetter(c))
            {
                int index = FindChar(c, ref gestureCharPool);
                if(index == -1)
                    return;
                SelectGesture(index);
            }
        }

        int FindChar(char c, ref char[] array)
        {
            int result = -1;
            for (int i=0; i<array.Length; ++i)
            {
                if (array[i].Equals(c))
                {
                    result = i;
                    break;
                }
            }
            return result;
        }

        void SelectIndex (int index)
        {
            // check for traps
            if (index == selectedIndex)
                return;

            if(index >= indexPanels.Length)
                return;

            // do the stuff
            

            selectedIndex = index;

            UpdateIndices();
            indexPanels[index].color = indexActiveColor;
        }

        public void setRecorded()
        {
            gesturesRecorded[selectedGesture] = gesturesRecorded[selectedGesture] | (int)Mathf.Pow(2, selectedIndex + 1);
        }

        void SelectGesture (int index)
        {
            // check for traps
            if (index == selectedGesture)
                return;

            if (index >= gesturePanels.Length)
                return;

            // do the stuff
            gesturePanels[index].color = gestureActiveColor;

            if (selectedGesture > -1)
                gesturePanels[selectedGesture].color = gestureColor;

            selectedGesture = index;
            selectedIndex = -1;
            UpdateIndices();
        }

        void UpdateIndices()
        {
            int n = gesturesRecorded[selectedGesture];
            // check, which of them is full
            for (int i=0; i<dataset.samplesPerGesture; ++i)
            {
                int number = (int)Mathf.Pow(2, i+1);
                if ((n & number) == number)
                {
                    // flag full
                    indexPanels[i].color = indexFullColor;
                }
                else
                {
                    indexPanels[i].color = indexColor;
                }
            }
        }
    }
}

