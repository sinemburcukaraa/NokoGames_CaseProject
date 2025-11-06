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
    [SerializeField] private int maxCapacity = 20;

    private List<GameObject> storedObjects = new();

    public bool HasAnyObject => storedObjects.Count > 0;
    public bool IsFull => storedObjects.Count >= maxCapacity;
    public int ObjectCount => storedObjects.Count;

    public bool CanAcceptObject(GameObject obj) => !IsFull;

    public void AddObject(GameObject obj)
    {
        if (!CanAcceptObject(obj)) return;

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
        if (!HasAnyObject) return null;

        GameObject top = storedObjects[^1];
        storedObjects.RemoveAt(storedObjects.Count - 1);
        top.transform.SetParent(null);
        return top;
    }
}
