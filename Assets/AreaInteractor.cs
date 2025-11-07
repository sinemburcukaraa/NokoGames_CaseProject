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
    private Dictionary<Transform, bool> interactorBusy = new();
    private Dictionary<Transform, Coroutine> activeCoroutines = new();
    private void Awake() => storage = GetComponent<AreaStorage>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("AI"))
        {
            interactorsInside.Add(other.transform);
            interactorBusy[other.transform] = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (interactorsInside.Contains(other.transform))
        {
            interactorsInside.Remove(other.transform);

            if (activeCoroutines.TryGetValue(other.transform, out Coroutine running))
            {
                StopCoroutine(running);
                activeCoroutines.Remove(other.transform);
            }

            interactorBusy.Remove(other.transform);
        }
    }

    public void Interact(Transform interactor)
    {
        if (!interactorsInside.Contains(interactor)) return;
        if (interactorBusy[interactor]) return;

        interactorBusy[interactor] = true;
        StartCoroutine(HandleInteraction(interactor));
    }

    private IEnumerator HandleInteraction(Transform interactor)
    {
        switch (areaType)
        {
            case AreaType.Input:
                ReceiveFromInteractor(interactor);
                break;

            case AreaType.Output:
                GiveObject(interactor);
                break;

            case AreaType.Trash:
                yield return StartCoroutine(SendToTrash(interactor));
                break;
        }

        var stack = interactor.GetComponent<StackSystem>();

        if (stack != null)
            yield return new WaitUntil(() => !stack.isProcessing);

        interactorBusy[interactor] = false;
    }

    private void GiveObject(Transform interactor)
    {
        if (!storage.HasAnyObject) return;
        var stackSystem = interactor.GetComponent<StackSystem>();
        if (stackSystem.IsFull)
            UIManager.Instance.ShowNotification("Your carrying capacity is full!");

        if (stackSystem == null || stackSystem.IsFull) return;

        GameObject topObject = storage.TakeTopObject();
        if (topObject == null) return;
        stackSystem.AddItem(topObject);
    }

    private void ReceiveFromInteractor(Transform interactor)
    {
        var stack = interactor.GetComponent<StackSystem>();
        if (stack == null || stack.IsEmpty) return;
        if (storage.IsFull || storage.isProcessing) return;

        GameObject obj = stack.PeekItem();
        if (obj == null) return;

        var resourceItem = obj.GetComponent<ResourceItem>();
        if (resourceItem == null) return;
        if (!storage.CanAcceptObject(resourceItem.gameObject)) return;

        obj = stack.RemoveItem();
        storage.AddObject(obj);
    }

    private IEnumerator SendToTrash(Transform interactor)
    {
        var stack = interactor.GetComponent<StackSystem>();
        if (stack == null || stack.IsEmpty)
            yield break;

        var trashMachine = GetComponentInParent<TrashStrategy>();
        if (trashMachine == null)
            yield break;

        Coroutine running = StartCoroutine(trashMachine.ProcessStack(stack));
        activeCoroutines[interactor] = running;

        yield return running;

        activeCoroutines.Remove(interactor);
        interactorBusy[interactor] = false;
    }
}

public enum AreaType
{
    Input,
    Output,
    Trash
}
