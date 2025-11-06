using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterBase : MonoBehaviour, IWorker
{
    // protected StackSystem stackSystem;
    // protected Transform currentTarget;

    // protected virtual void Awake()
    // {
    //     stackSystem = GetComponent<StackSystem>();
    // }

    // public abstract void MoveToTarget(Vector3 position);
    public void StartWork()
    {
        throw new System.NotImplementedException();
    }

    public void StopWork()
    {
        throw new System.NotImplementedException();
    }
}
