using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICharacterController : CharacterBase,IWorker
{
     public Transform spawner;
    public Transform transformer;

    private bool isWorking;

    public void StartWork()
    {
        isWorking = true;
        StartCoroutine(WorkRoutine());
    }

    public void StopWork()
    {
        isWorking = false;
        StopAllCoroutines();
    }

    private IEnumerator WorkRoutine()
    {
        yield return null;
        // while (isWorking)
        // {
        //     // 1. Spawner’dan ürün al
        //     MoveToTarget(spawner.position);
        //     yield return new WaitUntil(() => Vector3.Distance(transform.position, spawner.position) < 1f);
            
        //     GameObject item = spawner.GetComponent<SpawnerStrategy>().TakeItem();
        //     stackSystem.AddItem(item);

        //     // 2. Transformer’a taşı
        //     MoveToTarget(transformer.position);
        //     yield return new WaitUntil(() => Vector3.Distance(transform.position, transformer.position) < 1f);

        //     transformer.GetComponent<AssetProcessor>().ProcessItem(stackSystem.RemoveItem());
        // }
    }

    public override void MoveToTarget(Vector3 position)
    {
        transform.position = Vector3.MoveTowards(transform.position, position, 3f * Time.deltaTime);
    }
}
