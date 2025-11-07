using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICollectable
{
    void Collect();
}
public interface IInteractable
{
    void Interact(Transform interactor);
}

public interface IWorker
{
    void StartWork();
    void StopWork();
}
public interface IMachine
{
    void StartProcess();
    void StopProcess();
}
public interface IPoolable
{
    void ReturnToPool();
}