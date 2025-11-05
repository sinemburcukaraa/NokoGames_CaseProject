using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SpawnerPool : MonoBehaviour
{
    [Header("Spawner Settings")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform spawnerMachinePoint;
    [SerializeField] private GameObject prefab;
    [SerializeField] private int initialPoolSize = 10;
    [SerializeField] private float spawnDelay = 1f;

    private ObjectPool<Transform> objectPool;
    public event Action OnItemScaleCompleted;

    private void Awake()
    {
        objectPool = new ObjectPool<Transform>(prefab.transform, initialPoolSize, transform);
    }

    private void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnDelay);
            Transform obj = objectPool.Get();

            LaunchItem(obj);
        }
    }

    private void LaunchItem(Transform obj)
    {
        obj.position = spawnPoint.position;
        obj.rotation = UnityEngine.Quaternion.Euler(0, 90, 0);
        obj.gameObject.SetActive(true);

        obj.DOJump(spawnerMachinePoint.position, 2f, 1, 0.5f)
            .OnComplete(() =>
            {
                obj.DOScale(Vector3.zero, 1f).OnComplete(() =>
                {
                    obj.localScale = new Vector3(4f, 1.3f, 1.3f);
                    objectPool.ReturnToPool(obj);
                    OnItemScaleCompleted?.Invoke();

                });
            });
    }
}

