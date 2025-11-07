using System.Collections;
using UnityEngine;

public class SpawnerStrategy : MachineBase
{
    [Header("Spawner Settings")]
    [SerializeField]
    private Transform spawnPoint;

    [SerializeField]
    private GameObject prefab;

    [SerializeField]
    private int initialPoolSize = 10;

    private ObjectPool<Transform> pool;

    protected override void Awake()
    {
        base.Awake();
        pool = new ObjectPool<Transform>(prefab.transform, initialPoolSize, transform);
    }

    public override void StartProcess()
    {
        if (isProcessing || outputArea.IsFull)
            return;

        StartCoroutine(SpawnObjectsCoroutine());
    }

    private IEnumerator SpawnObjectsCoroutine()
    {
        isProcessing = true;

        int spawnCount = Mathf.Min(2, outputArea.maxCapacity - outputArea.ObjectCount);

        for (int i = 0; i < spawnCount; i++)
        {
            Transform obj = pool.Get();
            obj.position = spawnPoint.position + new Vector3(i * 0.3f, 0, 0);
            obj.rotation = Quaternion.Euler(0, 90, 0);
            obj.gameObject.SetActive(true);

            outputArea.AddObject(obj.gameObject);
            yield return new WaitUntil(() => !outputArea.isProcessing);
        }

        isProcessing = false;
    }


    public override IEnumerator ProcessItem(GameObject inputObject)
    {
        yield break;
    }
}
