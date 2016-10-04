using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{
    public MagicType m_magicTypeProjectile;
    protected Transform m_transWand;
    public virtual void Fire()
    {

    }

    public void SetWand(Transform _trans)
    {
        m_transWand = _trans;
    }

    public virtual void DestroyThis()
    {

    }

    public virtual float GetRange()
    {
        return -1;
    }
}
