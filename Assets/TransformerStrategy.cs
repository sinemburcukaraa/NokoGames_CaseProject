using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TransformerStrategy : AssetProcessor
{
    [Header("Positions")]
    [SerializeField]
    private Area inputArea; // Karakterten alınacak Area

    [SerializeField]
    private Area outputArea; // İşlem sonrası Area

    [SerializeField]
    private Transform processingPoint;

    [SerializeField]
    private GameObject processedPrefab;

    [Header("Jump / Scale")]
    [SerializeField]
    private float jumpHeight = 1f;

    [SerializeField]
    private float jumpDuration = 0.5f;

    private Queue<GameObject> processingQueue = new();
    private bool isProcessing = false;
    private ObjectPool<Transform> processedPool;
    public bool IsBusy => isProcessing;

    [Header("Pooling")]
    [SerializeField]
    private int poolSize = 10;

    public override void Interact(Transform interactor)
    {
        // var stack = interactor.GetComponent<StackSystem>();
        // if (stack == null || stack.Count == 0) return;

        // GameObject obj = stack.RemoveItem();
        // if (obj == null) return;

        // // Input Area'ya animasyonlu ekle

        // outputArea.PlaceInGrid(obj);

        // // İşlem kuyruğuna ekle
        // processingQueue.Enqueue(obj);

        // if (!isProcessing)
        //     StartCoroutine(ProcessQueue());
    }

    private void Awake()
    {
        // İşlem sonrası objeler için pool oluştur
        processedPool = new ObjectPool<Transform>(processedPrefab.transform, poolSize, transform);
    }

    private void Update()
    {
        if (!isProcessing && inputArea.HasAnyObject())
        {
            GameObject obj = inputArea.TakeTopObject();
            if (obj != null)
            {
                obj.SetActive(false); // input objesini gizle
                StartCoroutine(ProcessObject(obj));
            }
        }
    }

    private IEnumerator ProcessObject(GameObject inputObj)
    {
        isProcessing = true;

        // Processing noktası animasyonu (isteğe bağlı, input objesini göstermek için)
        inputObj.transform.gameObject.SetActive(true);
        yield return inputObj
            .transform.DOJump(processingPoint.position, jumpHeight, 1, jumpDuration)
            .SetEase(Ease.OutQuad)
            .WaitForCompletion();

        inputObj.SetActive(false); // İşlem sonrası input gizlensin

        yield return new WaitForSeconds(0.5f); // İşlem süresi

        // Pool’dan yeni objeyi al ve outputArea’ya ekle
        Transform newObj = processedPool.Get();
        newObj.gameObject.SetActive(true);
        outputArea.AddObject(newObj.gameObject); // Area içindeki PlaceInGrid otomatik çalışır

        isProcessing = false;
    }

    public GameObject TakeOutputItem()
    {
        if (outputArea.ObjectCount == 0)
            return null;

        GameObject obj = outputArea.TakeTopObject();
        return obj;
    }
}
