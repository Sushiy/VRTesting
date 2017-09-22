using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion_Absorb : MonoBehaviour
{
    private ParticleSystem explosion;

    public AnimationCurve m_animcurvePortalSize;

    public float m_fAbsorbtime = 1.0f;

    private void Start()
    {
        StartCoroutine(Absorb());
    }

    IEnumerator Absorb()
    {
        Debug.Log("Portal Despawning");
        float delta = 0.0f;
        while (delta < 1.0f)
        {
            delta += Time.deltaTime / m_fAbsorbtime;
            float curve = m_animcurvePortalSize.Evaluate(delta);
            transform.localScale = new Vector3(curve, curve, curve);
            yield return null;
        }

        yield return null;
    }
}
