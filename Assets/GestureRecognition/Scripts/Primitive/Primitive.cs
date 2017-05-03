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
        private int m_iTries = 3;
        [SerializeField]
        private TextMesh debug_text;

        private Transform m_thisTransform;
        private Vector3 debug_normal;

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

        public void setPosition(Vector3 normal, Vector3 center, float radius)
        {
            m_thisTransform.rotation = Quaternion.LookRotation(-normal);
            m_thisTransform.position = center;
            m_thisTransform.localScale = new Vector3(radius * m_fRadiusFactor,
                m_thisTransform.localScale.y, radius * m_fRadiusFactor);

            //bc of plane
            m_thisTransform.Rotate(new Vector3(-90f, 0));

            debug_normal = normal;
        }

        public void reduceTry(bool destroy)
        {
            m_iTries--;
            if (debug_text != null) debug_text.text = "Number of Tries: " + m_iTries;
            if (m_iTries == 0 || destroy)
            {
                Destroy(gameObject);
            }
        }

        void OnDrawGizmos()
        {
            if (debug_normal != null)
            {
                Gizmos.color = new Color(244/255f, 66/255f, 238/255f);
                Gizmos.DrawLine(m_thisTransform.position, debug_normal * 3f);
            }
        }
    }
}

