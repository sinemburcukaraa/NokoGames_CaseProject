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

            if (activeCoroutines.TryGetValue(other.transform, out Coroutine routine))
            {
                if (routine != null) StopCoroutine(routine);
                activeCoroutines.Remove(other.transform);
            }
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
                if (!activeCoroutines.ContainsKey(interactor))
                {
                    Coroutine c = StartCoroutine(GiveObjectsContinuously(interactor, 0.15f));
                    activeCoroutines.Add(interactor, c);
                }
                break;
        }
    }

    private IEnumerator GiveObjectsContinuously(Transform interactor, float delay)
    {
        if (!storage.CanAcceptObject(interactor.gameObject)) yield break; 
    
        var stackSystem = interactor.GetComponent<StackSystem>();
        if (stackSystem == null) yield break;

        while (interactorsInside.Contains(interactor)) 
        {
            while (storage.HasAnyObject)
            {
                GameObject topObject = storage.TakeTopObject();
                if (topObject == null) break;

                stackSystem.AddItem(topObject);
                yield return new WaitForSeconds(delay);
            }
            yield return null; // Yeni objeler için bekle
        }

        activeCoroutines.Remove(interactor);
    }
    private void ReceiveFromInteractor(Transform interactor)
    {
        var stack = interactor.GetComponent<StackSystem>();
        if (stack == null || stack.Count == 0) return;

        GameObject obj = stack.RemoveItem();
        if (obj == null) return;

        if (!storage.CanAcceptObject(obj))
        {
            stack.AddItem(obj);
            return;
        }

        storage.AddObject(obj);
    }

}
