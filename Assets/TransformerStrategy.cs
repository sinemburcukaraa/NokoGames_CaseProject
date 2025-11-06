using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TransformerStrategy : MonoBehaviour, IMachine
{
    [Header("Positions")]
    [SerializeField] private AreaStorage inputArea;
    [SerializeField] private AreaStorage outputArea;
    [SerializeField] private Transform processingPoint;
    [SerializeField] private GameObject processedPrefab;

    [Header("Jump / Scale")]
    [SerializeField] private float jumpHeight = 1f;
    [SerializeField] private float jumpDuration = 0.5f;

    [Header("Pooling")]
    [SerializeField] private int poolSize = 10;

    private Queue<GameObject> processingQueue = new();
    private bool isProcessing = false;
    private ObjectPool<Transform> processedPool;

    public bool IsBusy => isProcessing;

    private void Awake()
    {
        processedPool = new ObjectPool<Transform>(processedPrefab.transform, poolSize, transform);
    }
    public void Interact(Transform interactor)
    {
        if (!inputArea.HasAnyObject) return;

        GameObject obj = inputArea.TakeTopObject();
        if (obj == null) return;

        processingQueue.Enqueue(obj);
        if (!isProcessing)
            StartCoroutine(ProcessObject(obj));
    }
    private void Update()
    {
        if (!isProcessing && inputArea.HasAnyObject)
        {
            GameObject obj = inputArea.TakeTopObject();
            if (obj != null)
            {
                obj.SetActive(false);
                StartCoroutine(ProcessObject(obj));
            }
        }
    }

    private IEnumerator ProcessObject(GameObject inputObj)
    {
        isProcessing = true;

        inputObj.SetActive(true);
        yield return inputObj
            .transform.DOJump(processingPoint.position, jumpHeight, 1, jumpDuration)
            .SetEase(Ease.OutQuad)
            .WaitForCompletion();

        inputObj.SetActive(false);

        yield return new WaitForSeconds(0.5f); // işlem süresi

        Transform newObj = processedPool.Get();
        newObj.gameObject.SetActive(true);
        outputArea.AddObject(newObj.gameObject);

        isProcessing = false;
    }

    public GameObject TakeOutputItem()
    {
        if (outputArea.ObjectCount == 0) return null;
        return outputArea.TakeTopObject();
    }

    public void StartProcess() => isProcessing = true;
    public void StopProcess() => isProcessing = false;


}
