using UnityEngine;
using System.Collections;

namespace gesture
{
    /// <summary>
    /// Fills a given dataset with data.
    /// </summary>
    public class GestureDataRecorder : MonoBehaviour
    {
        [SerializeField]
        private GestureData dataset;
        [SerializeField]
        private GestureDataFlat flatDataset;
        [SerializeField]
        private GestureUI ui;
        [SerializeField]
        private bool useFlatDataset = false;

        private GestureConverter drawer;
        public GestureData Dataset {get { return dataset; } }
        public GestureDataFlat FlatDataset { get { return flatDataset; } }
        public bool UseFlatDataset { get { return useFlatDataset; } }

        void Awake()
        {
            drawer = GameObject.FindGameObjectWithTag("GestureObject").GetComponent<GestureConverter>();
        }

        void Start()
        {
            if (!useFlatDataset)
                dataset.Init();
            else
                flatDataset.Init();
        }

        /// <summary>
        /// TODO könnte noch ein bisschen besser gelöst werden können
        /// </summary>
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (!useFlatDataset)
                {
                    GestureObject g = new GestureObject();
                    g.type = (gestureTypes)ui.selectedGesture;
                    g.points = drawer.getCurrentGesture(dataset.pointsPerGesture); //TODO sollte sich das woanders herholen!
                    SetGesture(g);
                }
                else
                {
                    SetGesture(drawer.getCurrentGesture(flatDataset.pointsPerSample));
                }
            }
        }

        void SetGesture(Vector2[] samplePoints)
        {
            // check if gesture is too long
            if (samplePoints.Length != flatDataset.pointsPerSample)
                Debug.LogError("Careful! The gesture index count differs from the index cound needed from the dataset");

            if (ui.selectedIndex < 0)
                return;

            // copy the available points
            int indexOffset = (ui.selectedGesture * flatDataset.samplesPerGesture * flatDataset.pointsPerSample) +
                (ui.selectedIndex * flatDataset.pointsPerSample);
            for (int i=0; i<flatDataset.pointsPerSample; ++i)
            {
                flatDataset.gestures[indexOffset + i] = samplePoints[i];
            }

            // set dirty to save the data
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(flatDataset);
#endif
            ui.setRecorded();
        }

        void SetGesture(GestureObject g)
        {
            // check if the gesture is too long
            if (g.points.Length != dataset.pointsPerGesture)
                Debug.LogError("Careful! The gesture index count differs from the index count needed from the dataset");

            if (ui.selectedIndex < 0)
                return;

            int indexOffset = ui.selectedIndex * dataset.pointsPerGesture;

            for (int i=0; i<dataset.pointsPerGesture; ++i)
            {
                Vector2 value = g.points[i];
                Vector2[] list = dataset.gestures[ui.selectedGesture];
                list[indexOffset + i] = value;
            }
			#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(dataset);
			#endif
            ui.setRecorded();
        }
    }
}
