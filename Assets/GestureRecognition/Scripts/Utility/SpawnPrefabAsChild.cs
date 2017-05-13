using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPrefabAsChild : MonoBehaviour {

    [SerializeField]
    private GameObject prefabToSpawn;

	void Awake()
    {
        if (prefabToSpawn != null)
        {
            GameObject g = Instantiate<GameObject>(prefabToSpawn, transform);
        }
    }
}
