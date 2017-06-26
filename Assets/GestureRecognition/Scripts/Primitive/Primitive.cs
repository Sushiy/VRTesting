using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace primitive
{
    public class Primitive : MonoBehaviour {

        /* Static */
        public static int PrimitiveCount { get { return s_iPrimitiveCount; } }
        private static int s_iPrimitiveCount = 0;

        /* Member */
        [SerializeField]
        private float m_fRadiusFactor = 0.1f;
        [SerializeField]
        private int m_iTries = 1;
        [SerializeField]
        private TextMesh debug_text;
        private float m_fMaxTimeToCast = 3.0f;
        private float m_fTimer = 0.0f;

        private Transform m_thisTransform;

        public GameObject m_psDestruction;
        private bool m_bDestroying = false;

        void Awake()
        {
            m_thisTransform = GetComponent<Transform>();
            debug_text = GetComponentInChildren<TextMesh>();
        }

	    void Start () {
            s_iPrimitiveCount += 1;
	    }
	
	    void OnDestroy()
        {
            s_iPrimitiveCount -= 1;

        }

        void Deactivate()
        {
            GameObject go = GameObject.Instantiate(m_psDestruction);
            go.transform.rotation = m_thisTransform.rotation;
            go.transform.position = m_thisTransform.position;
            go.transform.localScale = new Vector3(m_thisTransform.localScale.x, m_thisTransform.localScale.x, m_thisTransform.localScale.x);
            Destroy(gameObject);
        }

        void Update()
        {
            if(!m_bDestroying)
                m_fTimer += Time.deltaTime;

            if (m_fTimer > m_fMaxTimeToCast)
            {
                Deactivate();
                m_bDestroying = true;
                m_fTimer = 0.0f;
            }

        }

        public void setPosition(Vector3 normal, Vector3 center, float radius)
        {
            m_thisTransform.rotation = Quaternion.LookRotation(-normal);
            m_thisTransform.position = center;
            m_thisTransform.localScale = new Vector3(radius * m_fRadiusFactor,
                m_thisTransform.localScale.y, radius * m_fRadiusFactor);

            //bc of plane
            m_thisTransform.Rotate(new Vector3(-90f, 0));
        }

        public void reduceTry(bool destroy)
        {
            m_iTries--;
            if (debug_text != null) debug_text.text = "Number of Tries: " + m_iTries;
            if (m_iTries == 0 || destroy)
            {
                Deactivate();
            }
        }
    }
}

