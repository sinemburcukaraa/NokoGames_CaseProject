using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterBase : MonoBehaviour
{
    protected StackSystem stackSystem;
    protected Transform currentTarget;

    protected virtual void Awake()
    {
        stackSystem = GetComponent<StackSystem>();
    }

    public abstract void MoveToTarget(Vector3 position);
}
