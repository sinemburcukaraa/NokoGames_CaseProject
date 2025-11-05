using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
public enum AreaType
{
    Input,  // Karakter objeleri bırakır
    Output  // Karakter objeleri alır
}

public class Area : MonoBehaviour, IInteractable
{
    [Header("Settings")]
    [SerializeField] private Transform stackPoint;
    [SerializeField] private AreaType areaType = AreaType.Output; // default Output
    [SerializeField] private int columns = 2;  // grid columns
    [SerializeField] private int rows = 2;     // grid rows
    [SerializeField] private float spacingX = 1f;
    [SerializeField] private float spacingZ = 1f;
    [SerializeField] private float layerHeight = 0.5f; // Y offset per layer
    [SerializeField] private float jumpHeight = 1f;
    [SerializeField] private float jumpDuration = 0.5f;

    private List<GameObject> storedObjects = new();
    private int nextGridIndex = 0;
    private bool isPlayerInside = false;
    [SerializeField] private int maxCapacity = 20; // maksimum obje sayısı

    // Event: Machine objeleri aldığında tetiklenir
    public event System.Action<GameObject> OnObjectTakenByMachine;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) isPlayerInside = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) isPlayerInside = false;
    }

    public void AddObject(GameObject obj)
    {
        // if (storedObjects.Count >= maxCapacity)
        // {
        //     Debug.Log("Area dolu! Yeni obje eklenemez.");
        //     return;
        // }
        storedObjects.Add(obj);
        PlaceInGrid(obj);
    }
    public bool IsFull() => storedObjects.Count >= maxCapacity;

    public void PlaceInGrid(GameObject obj)
    {
        Vector3 targetPos = CalculateGridPosition();
        // obj.transform.position = stackPoint.position + new Vector3(-.4f, 0, -.4f);
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
        int z = indexInLayer / columns % rows; // rows ile sınırlı

        Vector3 startPos = stackPoint.position + new Vector3(-.4f, 0, -.4f);
        Vector3 targetPos = startPos + new Vector3(x * spacingX, currentLayer * layerHeight, z * spacingZ);
        return targetPos;

    }
    public GameObject TakeTopObject()
    {
        if (storedObjects.Count == 0) return null;
        GameObject top = storedObjects[^1];
        storedObjects.RemoveAt(storedObjects.Count - 1);
        top.transform.SetParent(null);

        nextGridIndex = Mathf.Max(nextGridIndex - 1, 0); // indexi azalt
        return top;
    }

    public bool HasAnyObject() => storedObjects.Count > 0;

    public int ObjectCount => storedObjects.Count;

    public void Interact(Transform interactor)
    {
        if (!isPlayerInside) return;

        switch (areaType)
        {
            case AreaType.Input:
                ReceiveFromPlayer(interactor);
                break;
            case AreaType.Output:
                GiveToPlayer(interactor);
                break;
        }
    }
    // public void AddObjectWithJump(GameObject obj, float jumpHeight = 1f, float duration = 0.5f)
    // {
    //     storedObjects.Add(obj);

    //     // Başlangıç pozisyonunu objenin mevcut pozisyonuna al
    //     Vector3 startPos = obj.transform.position;

    //     // Hedef pozisyonu grid hesaplayarak al
    //     int itemsPerLayer = columns * rows;
    //     int currentLayer = ObjectCount / itemsPerLayer;
    //     int indexInLayer = ObjectCount % itemsPerLayer;

    //     int x = indexInLayer % columns;
    //     int z = indexInLayer / columns;

    //     Vector3 targetPos = stackPoint.position + new Vector3(x * spacingX, currentLayer * layerHeight, z * spacingZ);

    //     obj.transform.SetParent(stackPoint);
    //     obj.SetActive(true);

    //     // Jump animasyonu
    //     obj.transform.DOJump(targetPos, jumpHeight, 1, duration).SetEase(Ease.OutQuad)
    //         .OnComplete(() =>
    //         {
    //             // Son pozisyonu kesin olarak hedefe ayarla
    //             obj.transform.localPosition = targetPos - stackPoint.position;
    //         });
    // }
    private void ReceiveFromPlayer(Transform interactor)
    {
        var stack = interactor.GetComponent<StackSystem>();
        if (stack == null || stack.Count == 0) return;

        GameObject obj = stack.RemoveItem();
        if (obj == null) return;

        AddObject(obj);
    }

    private void GiveToPlayer(Transform interactor)
    {
        StartCoroutine(GiveObjectsWithDelay(interactor, 0.15f));
    }

    private IEnumerator GiveObjectsWithDelay(Transform interactor, float delay)
    {
        var stackSystem = interactor.GetComponent<StackSystem>();
        if (stackSystem == null)
        {
            Debug.LogWarning("Interactor’da StackSystem yok!");
            yield break;
        }

        while (HasAnyObject()) // artık isPlayerInside kontrolü yok
        {
            GameObject topObject = TakeTopObject();
            if (topObject == null) yield break;

            stackSystem.AddItem(topObject);
            yield return new WaitForSeconds(delay);
        }
    }
    // Machine çağırabilir
    public void MachineTakeObject()
    {
        if (!HasAnyObject()) return;

        GameObject top = TakeTopObject();
        if (top == null) return;

        OnObjectTakenByMachine?.Invoke(top);
    }

}