using System.Collections;
using UnityEngine;

public class TransformerStrategy : MachineBase
{
    [Header("Transformer Settings")]
    [SerializeField]
    private GameObject processedPrefab;

    [SerializeField]
    private int poolSize = 10;

    private ObjectPool<Transform> processedPool;

    protected override void Awake()
    {
        base.Awake();
        processedPool = new ObjectPool<Transform>(processedPrefab.transform, poolSize, transform);
    }

    protected override IEnumerator ProcessItem(GameObject inputObject)
    {
        yield return new WaitForSeconds(0.5f);

        Transform newObj = processedPool.Get();
        newObj.gameObject.SetActive(true);

        inputArea.AddObject(newObj.gameObject);
    }
}
