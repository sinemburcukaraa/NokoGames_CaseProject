using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ResourceItem : MonoBehaviour
{
    public ResourceType Type;
    [HideInInspector] public ObjectPool<Transform> originPool;
    
}
public enum ResourceType
{
    None,
    refined,
    unrefined,
}