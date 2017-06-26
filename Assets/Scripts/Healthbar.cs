using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healthbar : MonoBehaviour {

    [SerializeField]
    [Range(0f,1f)]
    private float m_fHealthfactor = 1f;
    [SerializeField]
    private MP_Health m_MPhealth;

    private Transform[] m_arrFireTrans;
    private int m_iNumFires;
    private float m_fFullScaleOfFire;

    private void Awake()
    {
        m_arrFireTrans = GetComponentsInChildren<Transform>();
        m_iNumFires = m_arrFireTrans.Length -1;
        m_fFullScaleOfFire = m_arrFireTrans[0].localScale.x;
    }

    private void Update() { 
    //public void UpdateHealth (int currentHealth) {
        if (m_MPhealth != null)
        {
            //m_fHealthfactor = (float)currentHealth / MP_Health.MAX_HEALTH;
            m_fHealthfactor = (float)m_MPhealth.currentHealth / MP_Health.MAX_HEALTH;
        }

        float scale = Mathf.Lerp(0f, m_iNumFires, m_fHealthfactor);
		for (int i=1; i<=m_iNumFires; ++i)
        {
            int wholePart = Mathf.FloorToInt(scale);
            float decPart = scale - wholePart;
            float factor = 1f;
            if (i-1 == wholePart) factor = decPart;
            else if (i-1 > wholePart) factor = 0f;
            setFactorInFire(i, factor);
        }
	}

    void setFactorInFire(int index, float factor)
    {
        float scaleFactor = Mathf.Lerp(0f, m_fFullScaleOfFire, factor);
        Vector3 newScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
        m_arrFireTrans[index].localScale = newScale;
    }
}
