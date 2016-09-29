using UnityEngine;
using System.Collections;

public class Projectiles : MonoBehaviour
{
    protected Transform m_transWand;
    public virtual void Fire()
    {

    }

    public void SetWand(Transform _trans)
    {
        m_transWand = _trans;
    }

    public virtual float GetRange()
    {
        return -1;
    }
}
