using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SpawnerPool : MonoBehaviour
{
    [Header("Spawner Settings")]
    [SerializeField]
    private Transform spawnPoint;

    [SerializeField]
    private Transform spawnerMachinePoint;

    [SerializeField]
    private GameObject prefab;

    [SerializeField]
    private int initialPoolSize = 5;

    [SerializeField]
    private float spawnDelay = 1f;

    [SerializeField]
    private SpawnerStrategy spawnerStrategy;

    [SerializeField]
    private AreaStorage targetArea;

    private ObjectPool<Transform> objectPool;
    private Coroutine spawnCoroutine;

    private void Awake()
    {
        objectPool = new ObjectPool<Transform>(prefab.transform, initialPoolSize, transform);
    }

    private void OnEnable()
    {
        if (targetArea != null)
        {
            targetArea.OnAreaFilled += StopSpawning;
            targetArea.OnAreaBecameAvailable += StartSpawning;
        }
    }

    private void OnDisable()
    {
        if (targetArea != null)
        {
            targetArea.OnAreaFilled -= StopSpawning;
            targetArea.OnAreaBecameAvailable -= StartSpawning;
        }
    }

    private void Start()
    {
        StartSpawning();
    }

    private void StartSpawning()
    {
        if (spawnCoroutine == null)
            spawnCoroutine = StartCoroutine(SpawnRoutine());
    }

    private void StopSpawning()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            if (targetArea != null && targetArea.IsFull)
            {
                StopSpawning();
                yield break;
            }

            yield return new WaitForSeconds(spawnDelay);

            Transform obj = objectPool.Get();
            LaunchItem(obj);
        }
    }

    private void LaunchItem(Transform obj)
    {
        obj.position = spawnPoint.position;
        obj.rotation = Quaternion.Euler(0, 90, 0);
        obj.localScale = new Vector3(4, 1.3f, 1.3f);
        obj.gameObject.SetActive(true);

        obj.DOJump(spawnerMachinePoint.position, 2f, 1, 0.5f)
            .OnComplete(() =>
            {
                obj.DOScale(Vector3.zero, 1f)
                    .OnComplete(() =>
                    {
                        objectPool.ReturnToPool(obj);

                        if (spawnerStrategy != null && !targetArea.IsFull)
                        {
                            spawnerStrategy.StartProcess();
                        }
                    });
            });
    }
}
