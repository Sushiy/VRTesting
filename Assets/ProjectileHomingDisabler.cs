using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileHomingDisabler : MonoBehaviour {

    public LayerMask mask;
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == mask)
        {
            Debug.Log("Projectile entered");
            PhysicsHoming homing = other.gameObject.GetComponent<PhysicsHoming>();
            if (homing != null)
                homing.enabled = false;
        }
    }
}
