using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SpawnerStrategy : AssetProcessor
{
    [Header("Spawner Settings")]
    [SerializeField]
    private Transform spawnPoint;

    [SerializeField]
    private Transform machinePoint;

    [SerializeField]
    private GameObject prefab;

    [SerializeField]
    private int initialPoolSize = 10;

    [SerializeField]
    private float spawnDelay = 1f;

    [Header("Grid Settings")]
    [SerializeField]
    private Transform outputGridParent;

    [SerializeField]
    private int columns = 2;

    [SerializeField]
    private int rows = 2;

    [SerializeField]
    private float spacingX = 1f;

    [SerializeField]
    private float spacingY = 1f;

    [Header("Jump / Scale")]
    [SerializeField]
    private float jumpHeight = 2f;

    [SerializeField]
    private float jumpDuration = 0.5f;

    private ObjectPool<Transform> pool;
    private readonly Queue<GameObject> activeItems = new();

    private int nextGridIndex = 0;

    public event Action<GameObject> OnItemLaunched;
    [SerializeField] private SpawnerPool spawnerPool;

    private void Awake()
    {
        pool = new ObjectPool<Transform>(prefab.transform, initialPoolSize, transform);
    }
    public override void Interact(Transform interactor)
    {
        if (activeItems.Count == 0)
            return;

        var stack = interactor.GetComponent<StackSystem>();
        if (stack == null)
            return;

        GameObject obj = activeItems.Dequeue();
        obj.SetActive(false);
        stack.AddItem(obj);
        pool.ReturnToPool(obj.transform);
    }

    public override void StartProcess()
    {
        for (int i = 0; i < 2; i++)
        {
            Transform obj = pool.Get();
            obj.position = spawnPoint.position + new Vector3(i * 0.3f, 0, 0);
            obj.rotation = Quaternion.Euler(0, 90, 0);
            obj.gameObject.SetActive(true);

            PlaceInGrid(obj.gameObject);
            activeItems.Enqueue(obj.gameObject);
        }
    }
    private void PlaceInGrid(GameObject obj)
    {
        int itemsPerLayer = columns * rows;
        int currentLayer = nextGridIndex / itemsPerLayer;
        int indexInLayer = nextGridIndex % itemsPerLayer;

        int x = indexInLayer % columns;
        int y = indexInLayer / columns;

        Vector3 startPos = outputGridParent.position + new Vector3(-.3f, 0, -.3f);
        Vector3 targetPos = startPos + new Vector3(x * spacingX, currentLayer * .3f, y * spacingY);

        obj.transform.DOJump(targetPos, jumpHeight, 1, jumpDuration).SetEase(Ease.OutQuad);

        nextGridIndex++;
    }
    private void OnEnable()
    {
        if (spawnerPool != null)
            spawnerPool.OnItemScaleCompleted += StartProcess;
    }
    private void OnDisable()
    {
        if (spawnerPool != null)
            spawnerPool.OnItemScaleCompleted -= StartProcess;
    }


}
