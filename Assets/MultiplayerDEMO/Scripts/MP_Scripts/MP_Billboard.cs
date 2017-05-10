using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MP_Billboard : MonoBehaviour
{
    void Update()
    {
        if(Camera.main != null)
            transform.LookAt(Camera.main.transform);
    }
}
