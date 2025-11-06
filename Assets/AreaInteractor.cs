using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(AreaStorage))]
public class AreaInteractor : MonoBehaviour, IInteractable
{
    [SerializeField] private AreaType areaType = AreaType.Output;
    private AreaStorage storage;
    private HashSet<Transform> interactorsInside = new();
    private Dictionary<Transform, Coroutine> activeCoroutines = new();

    private void Awake() => storage = GetComponent<AreaStorage>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("AI"))
        {
            interactorsInside.Add(other.transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (interactorsInside.Contains(other.transform))
        {
            interactorsInside.Remove(other.transform);
        }
    }

    public void Interact(Transform interactor)
    {
        if (!interactorsInside.Contains(interactor)) return;

        switch (areaType)
        {
            case AreaType.Input:
                ReceiveFromInteractor(interactor);
                break;

            case AreaType.Output:
                GiveObject(interactor);
                break;

            case AreaType.Trash:
                SendToTrash(interactor);
                break;
        }
    }
    private void SendToTrash(Transform interactor)
    {
        var stack = interactor.GetComponent<StackSystem>();
        if (stack == null || stack.IsEmpty) return;

        var trashMachine = GetComponentInParent<TrashStrategy>();
        if (trashMachine == null)
        {
            Debug.LogWarning("TrashStrategy component bulunamadı!");
            return;
        }

        StartCoroutine(trashMachine.ProcessStack(stack));
    }


    private void GiveObject(Transform interactor)
    {
        if (!storage.HasAnyObject) return;

        var stackSystem = interactor.GetComponent<StackSystem>();
        if (stackSystem == null) return;

        if (stackSystem.IsFull)
            return;

        GameObject topObject = storage.TakeTopObject();
        if (topObject == null) return;

        stackSystem.AddItem(topObject);
    }

    private void ReceiveFromInteractor(Transform interactor)
    {
        var stack = interactor.GetComponent<StackSystem>();
        if (stack == null || stack.Count == 0) return;

        if (storage.IsFull)
            return;

        GameObject obj = stack.PeekItem();
        if (obj == null) return;

        var resourceItem = obj.GetComponent<ResourceItem>();
        if (resourceItem == null)
            return;

        if (!storage.CanAcceptObject(resourceItem.gameObject))
        {
            return;
        }

        obj = stack.RemoveItem();
        storage.AddObject(obj);
    }

}
public enum AreaType
{
    Input,
    Output,
    Trash
}