using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public enum AreaType
{
    Input,
    Output,
}

[RequireComponent(typeof(Collider))]
public class Area : MonoBehaviour, IInteractable
{
    [Header("Settings")]
    [SerializeField]
    private Transform stackPoint;

    [SerializeField]
    private AreaType areaType = AreaType.Output;

    [SerializeField]
    private int columns = 2;

    [SerializeField]
    private int rows = 2;

    [SerializeField]
    private float spacingX = 1f;

    [SerializeField]
    private float spacingZ = 1f;

    [SerializeField]
    private float layerHeight = 0.5f;

    [SerializeField]
    private float jumpHeight = 1f;

    [SerializeField]
    private float jumpDuration = 0.5f;

    [SerializeField]
    private int maxCapacity = 20;

    [Header("Accepted Object Tags (Optional)")]
    [SerializeField]
    private List<string> acceptedTags = new();

    private List<GameObject> storedObjects = new();
    private int nextGridIndex = 0;
    private bool isPlayerInside = false;

    public event System.Action<GameObject> OnObjectTakenByMachine;

    #region Trigger
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            isPlayerInside = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            isPlayerInside = false;
    }
    #endregion

    #region Object Management
    public bool IsFull() => storedObjects.Count >= maxCapacity;

    public bool HasAnyObject() => storedObjects.Count > 0;

    public int ObjectCount => storedObjects.Count;

    public bool CanAcceptObject(GameObject obj)
    {
        return !IsFull() && (acceptedTags.Count == 0 || acceptedTags.Contains(obj.tag));
    }

    public void AddObject(GameObject obj)
    {
        if (!CanAcceptObject(obj))
            return;

        storedObjects.Add(obj);
        PlaceInGrid(obj);
    }

    private void PlaceInGrid(GameObject obj)
    {
        Vector3 targetPos = CalculateGridPosition();
        obj.transform.rotation = Quaternion.Euler(0, 90, 0);
        obj.SetActive(true);
        obj.transform.SetParent(stackPoint);

        obj.transform.DOJump(targetPos, jumpHeight, 1, jumpDuration).SetEase(Ease.OutQuad);
        nextGridIndex++;
    }

    private Vector3 CalculateGridPosition()
    {
        int index = storedObjects.Count - 1;
        int itemsPerLayer = columns * rows;
        int currentLayer = index / itemsPerLayer;
        int indexInLayer = index % itemsPerLayer;

        int x = indexInLayer % columns;
        int z = indexInLayer / columns % rows;

        Vector3 startPos = stackPoint.position + new Vector3(-0.4f, 0, -0.4f);
        return startPos + new Vector3(x * spacingX, currentLayer * layerHeight, z * spacingZ);
    }

    public GameObject TakeTopObject()
    {
        if (!HasAnyObject())
            return null;

        GameObject top = storedObjects[^1];
        storedObjects.RemoveAt(storedObjects.Count - 1);
        top.transform.SetParent(null);

        nextGridIndex = Mathf.Max(nextGridIndex - 1, 0);
        return top;
    }
    #endregion

    #region Interaction
    public void Interact(Transform interactor)
    {

        if (!isPlayerInside)
            return;

        switch (areaType)
        {
            case AreaType.Input:
                ReceiveFromInteractor(interactor);
                break;
            case AreaType.Output:
                StartCoroutine(GiveObjectsWithDelay(interactor, 0.15f));
                break;
        }
    }

    private void ReceiveFromInteractor(Transform interactor)
    {

        var stack = interactor.GetComponent<StackSystem>();
        if (stack == null || stack.Count == 0) return;

        GameObject obj = stack.RemoveItem();
        if (obj == null) return;

        if (!CanAcceptObject(obj))
        {
            stack.AddItem(obj);
            return;
        }

        AddObject(obj);
    }

    private IEnumerator GiveObjectsWithDelay(Transform interactor, float delay)
    {
        var stackSystem = interactor.GetComponent<StackSystem>();
        if (stackSystem == null) yield break;

        while (HasAnyObject())
        {
            GameObject topObject = TakeTopObject();
            if (topObject == null)
                yield break;

            stackSystem.AddItem(topObject);
            yield return new WaitForSeconds(delay);
        }
    }

    // Makine çağrabilir
    public void MachineTakeObject()
    {
        if (!HasAnyObject())
            return;

        GameObject top = TakeTopObject();
        if (top == null)
            return;

        OnObjectTakenByMachine?.Invoke(top);
    }
    #endregion
}
