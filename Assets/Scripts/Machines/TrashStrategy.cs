using System.Collections;
using DG.Tweening;
using UnityEngine;

public class TrashStrategy : MachineBase
{
    [Header("Trash Machine Settings")]
    [SerializeField] private float destroyDelay = 0.2f;
    [SerializeField] private bool usePooling = true;
    [SerializeField] private float jumpPower = 0.5f;


    public override IEnumerator ProcessItem(GameObject inputObject)
    {
        if (inputObject == null)
            yield break;

        yield return new WaitForSeconds(destroyDelay);

        ReturnToPoolIfPossible(inputObject);
    }
    public IEnumerator ProcessStack(StackSystem stack)
    {
        if (stack == null || stack.IsEmpty) yield break;

        while (!stack.IsEmpty)
        {
            GameObject obj = stack.RemoveItem();
            if (obj == null) yield break;

            obj.transform.SetParent(null);
            obj.SetActive(true);

            obj.transform.DOJump(processingPoint.position, jumpPower, 1, jumpDuration);

            StartCoroutine(ProcessItem(obj));

            yield return new WaitForSeconds(.1f);
        }
    }


    private void ReturnToPoolIfPossible(GameObject obj)
    {
        var resourceItem = obj.GetComponent<ResourceItem>();
        if (resourceItem != null && usePooling)
        {
            if (resourceItem.originPool != null)
            {
                resourceItem.originPool.ReturnToPool(resourceItem.transform);
                return;
            }
        }
        Destroy(obj);
    }
}
