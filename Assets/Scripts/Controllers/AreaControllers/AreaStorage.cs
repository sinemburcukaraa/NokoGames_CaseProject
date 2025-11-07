using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class AreaStorage : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private Transform stackPoint;
    [SerializeField] private int columns = 2;
    [SerializeField] private int rows = 2;
    [SerializeField] private float spacingX = 0.5f;
    [SerializeField] private float spacingZ = 0.5f;
    [SerializeField] private float layerHeight = 0.3f;

    [Header("Animation Settings")]
    [SerializeField] private float jumpHeight = 1f;
    [SerializeField] private float jumpDuration = 0.5f;

    [Header("Capacity")]
    public int maxCapacity = 20;

    [SerializeField] private List<GameObject> storedObjects = new();

    [Header("Accepted Resource Types")]
    public ResourceType acceptedType = ResourceType.None;

    public event Action OnAreaBecameAvailable;
    public event Action OnAreaFilled;
    public event Action OnObjectAdded;
    public event Action OnObjectRemoved;

    public bool HasAnyObject => storedObjects.Count > 0;
    public bool IsFull => storedObjects.Count >= maxCapacity;
    public int ObjectCount => storedObjects.Count;
    public bool IsEmpty => storedObjects.Count == 0;

    private Queue<GameObject> objectQueue = new();
    private Coroutine processingCoroutine = null;
    public bool isProcessing = false;

    public bool CanAcceptObject(GameObject obj)
    {
        if (IsFull) return false;

        var resource = obj.GetComponent<ResourceItem>();
        if (resource == null) return false;

        return acceptedType.HasFlag(ResourceType.All) || acceptedType.HasFlag(resource.Type);
    }

    public void AddObject(GameObject obj)
    {
        if (!CanAcceptObject(obj))
            return;

        objectQueue.Enqueue(obj);

        if (processingCoroutine == null)
            processingCoroutine = StartCoroutine(ProcessQueue());
    }

    private IEnumerator ProcessQueue()
    {
        isProcessing = true;

        while (objectQueue.Count > 0)
        {
            GameObject obj = objectQueue.Dequeue();
            if (obj == null) continue;

            storedObjects.Add(obj);

            Vector3 targetPos = CalculateGridPosition();
            obj.transform.rotation = Quaternion.Euler(0, 90, 0);
            obj.SetActive(true);
            obj.transform.SetParent(stackPoint);

            yield return obj.transform.DOJump(targetPos, jumpHeight, 1, jumpDuration)
                .SetEase(Ease.OutQuad);
            yield return new WaitForSeconds(0.05f); 

            OnObjectAdded?.Invoke();
            if (IsFull)
            {
                OnAreaFilled?.Invoke();
                UIManager.Instance.ShowNotification("Storage is Full!!");
            }
        }

        isProcessing = false;
        processingCoroutine = null;
    }

    public GameObject TakeTopObject()
    {
        if (!HasAnyObject) return null;

        GameObject top = storedObjects[^1];
        if (top == null || top.transform == null)
            return null;

        storedObjects.RemoveAt(storedObjects.Count - 1);
        top.transform.SetParent(null);

        if (!IsFull)
            OnAreaBecameAvailable?.Invoke();
        OnObjectRemoved?.Invoke();

        return top;
    }

    public void StopProcessing()
    {
        if (processingCoroutine != null)
        {
            StopCoroutine(processingCoroutine);
            processingCoroutine = null;
            isProcessing = false;
        }

        objectQueue.Clear(); 
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
}
