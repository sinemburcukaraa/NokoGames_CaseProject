using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ResourceItem : MonoBehaviour, IPoolable
{
    public ResourceType Type;
    [HideInInspector] public ObjectPool<Transform> originPool;

    public void ReturnToPool()
    {
        if (originPool != null)
        {
            originPool.ReturnToPool(transform);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
[System.Flags]
public enum ResourceType
{
    None = 0,
    Unrefined = 1 << 0,
    Refined = 1 << 1,
    All = Unrefined | Refined
}