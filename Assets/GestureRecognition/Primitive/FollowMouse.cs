using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace primitive
{
    /// <summary>
    /// This object will follow the mouse
    /// </summary>
    public class FollowMouse : MonoBehaviour
    {

        [SerializeField]
        private float m_fDepth = 3f;
        private Transform m_Transform;

        void Awake()
        {
            m_Transform = transform;
        }

        void Update()
        {
            Vector3 mousePos = Input.mousePosition;
            transform.position = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, m_fDepth));
        }
    }

}
