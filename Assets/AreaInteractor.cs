using System.Collections;
using System.Collections.Generic;
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
        }
    }
    private void GiveObject(Transform interactor)
    {
        if (!storage.HasAnyObject) return;

        var stackSystem = interactor.GetComponent<StackSystem>();
        if (stackSystem == null) return;

        // 🔒 Eğer stack doluysa hiç işlem yapma
        if (stackSystem.IsFull)
            return;

        // Artık güvenle objeyi alabiliriz
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
