using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpellRegistry", menuName = "Spells", order = 1)]
public class SpellRegistry : ScriptableObject
{
    public GameObject[] serverPrefabs;
    public GameObject[] clientPrefabs;
}
