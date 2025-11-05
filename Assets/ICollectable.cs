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
public interface IProcessorStrategy
{
    void Process(GameObject input);
}
public abstract class AssetProcessor : MonoBehaviour, IInteractable
{
    [SerializeField] protected Transform inputPoint;
    [SerializeField] protected Transform outputPoint;
    protected bool isWorking;

    public abstract void Interact(Transform interactor);

    public virtual void StartProcess() => isWorking = true;
    public virtual void StopProcess() => isWorking = false;
}
