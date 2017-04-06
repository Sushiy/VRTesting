using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MP_PlayerController : MonoBehaviour
{
    void Update()
    {
        float x = Input.GetAxis("Horizontal") * Time.deltaTime * 150.0f;
        float z = Input.GetAxis("Vertical") * Time.deltaTime * 3.0f;

        transform.Rotate(0, x, 0);
        transform.Translate(0, 0, z);
    }
}
