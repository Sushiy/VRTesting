using UnityEngine;
using System.Collections;

namespace gesture
{
    public class GestureRecorder : MonoBehaviour
    {
        public GestureData dataset;
        public GestureUI ui;

        private GestureDrawer drawer;

        void Awake()
        {
            drawer = GameObject.FindGameObjectWithTag("GestureObject").GetComponent<GestureDrawer>();
        }

        void Start()
        {
            dataset.Init();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                gesture g = new gesture();
                g.type = (gestureTypes)ui.selectedGesture;
                g.points = drawer.getCurrentGesture();
                SetGesture(g);
            }
        }

        void SetGesture(gesture g)
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
