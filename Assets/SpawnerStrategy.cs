using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SpawnerStrategy : MonoBehaviour, IMachine
{
    [Header("Spawner Settings")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject prefab;
    [SerializeField] private int initialPoolSize = 10;

    [Header("Output Area")]
    [SerializeField] private AreaStorage areaStorage;

    private ObjectPool<Transform> pool;
    private bool isProcessing = false;

    private void Awake()
    {
        pool = new ObjectPool<Transform>(prefab.transform, initialPoolSize, transform);
    }

    public void StartProcess()
    {
        if (isProcessing) return;
        isProcessing = true;

        for (int i = 0; i < 2; i++)
        {
            Transform obj = pool.Get();
            obj.position = spawnPoint.position + new Vector3(i * 0.3f, 0, 0);
            obj.rotation = Quaternion.Euler(0, 90, 0);
            obj.gameObject.SetActive(true);

            areaStorage.AddObject(obj.gameObject);
        }

        isProcessing = false;
    }

    public void StopProcess()
    {
        isProcessing = false;
    }
}
