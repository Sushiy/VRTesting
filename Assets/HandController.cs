using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour {

    [SerializeField]
    private float m_fSpeed = 30f;

    private Transform m_Transform;
    private Transform m_wandTransform;

    void Awake()
    {
        m_Transform = GetComponent<Transform>();
        m_wandTransform = m_Transform.GetChild(0);
    }

	void Update () {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 rotation = m_Transform.rotation.eulerAngles;
        rotation += new Vector3(-v * m_fSpeed * Time.deltaTime, h * m_fSpeed * Time.deltaTime, 0);
        m_Transform.rotation = Quaternion.Euler(rotation);

        if (Input.GetKeyDown(KeyCode.Space))
            m_wandTransform.localPosition = new Vector3(0, 0, 10);
        else if (Input.GetKeyUp(KeyCode.Space))
            m_wandTransform.localPosition = new Vector3(0, 0, 5);
    }
}
