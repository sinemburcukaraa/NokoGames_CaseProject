using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SpawnerStrategy : AssetProcessor
{
    [Header("Spawner Settings")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject prefab;
    [SerializeField] private int initialPoolSize = 10;


    [Header("Output Area")]
    [SerializeField] private Area outputArea;

    private ObjectPool<Transform> pool;
    [SerializeField] private SpawnerPool spawnerPool;

    private void Awake()
    {
        pool = new ObjectPool<Transform>(prefab.transform, initialPoolSize, transform);
    }

    public override void StartProcess()
    {
        for (int i = 0; i < 2; i++)
        {
            Transform obj = pool.Get();
            obj.position = spawnPoint.position + new Vector3(i * 0.3f, 0, 0);
            obj.rotation = Quaternion.Euler(0, 90, 0);
            obj.gameObject.SetActive(true);

            outputArea.PlaceInGrid(obj.gameObject);

            outputArea.AddObject(obj.gameObject);
        }
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

    public override void Interact(Transform interactor)
    {
        throw new NotImplementedException();
    }
}
