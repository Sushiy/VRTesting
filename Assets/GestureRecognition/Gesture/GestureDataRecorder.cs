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
        private GestureUI ui;

        private GestureConverter drawer;
        public GestureData Dataset {get { return dataset; } }

        void Awake()
        {
            drawer = GameObject.FindGameObjectWithTag("GestureObject").GetComponent<GestureConverter>();
        }

        void Start()
        {
            dataset.Init();
        }

        /// <summary>
        /// TODO könnte noch ein bisschen besser gelöst werden können
        /// </summary>
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                GestureObject g = new GestureObject();
                g.type = (gestureTypes)ui.selectedGesture;
                g.points = drawer.getCurrentGesture(); //TODO sollte sich das woanders herholen!
                SetGesture(g);
            }
        }

        void SetGesture(GestureObject g)
        {
            // check if the gesture is too long
            if (g.points.Length != dataset.pointsPerGesture)
                print("Careful! The gesture index count differs from the index count needed from the dataset");

            if (ui.selectedIndex < 0)
                return;

            int indexOffset = ui.selectedIndex * dataset.samplesPerGesture;

            for (int i=0; i<dataset.pointsPerGesture; ++i)
            {
                Vector2 value = g.points[i];
                Vector2[] list = dataset.gestures[ui.selectedGesture];
                list[indexOffset + i] = value;
            }

            ui.setRecorded();
        }
    }
}
