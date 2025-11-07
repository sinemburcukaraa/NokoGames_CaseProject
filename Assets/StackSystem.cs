using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class StackSystem : MonoBehaviour
{
    [Header("Stack Settings")]
    public Transform stackParent;
    public float itemHeight = 0.3f;
    public float jumpPower = 0.3f;
    public float moveDuration = 0.25f;
    public float swayAmount = 0.03f;
    [SerializeField] private int maxCapacity = 10;
    public List<GameObject> stackedItems = new List<GameObject>();

    public int Count => stackedItems.Count;
    public bool IsFull => stackedItems.Count >= maxCapacity;
    public bool IsEmpty => stackedItems.Count == 0;

    public bool isProcessing = false;

    public void AddItem(GameObject item)
    {
        if (IsFull || isProcessing) return;

        stackedItems.Add(item);
        item.transform.SetParent(stackParent, true);

        Vector3 targetPos = Vector3.up * itemHeight * (stackedItems.Count - 1);
        item.transform.localRotation = Quaternion.identity;
        item.transform.DOKill();

        StartCoroutine(AnimateItem(item, targetPos));
    }

    private IEnumerator AnimateItem(GameObject item, Vector3 targetPos)
    {
        isProcessing = true; // Başladı
        yield return item.transform.DOLocalJump(targetPos, jumpPower, 1, moveDuration)
            .SetEase(Ease.OutQuad)
            .WaitForCompletion();
        item.transform.localPosition = targetPos;
        isProcessing = false; // Bitti
    }


    public GameObject RemoveItem()
    {
        if (IsEmpty) return null;

        GameObject item = stackedItems[^1];
        stackedItems.RemoveAt(stackedItems.Count - 1);

        item.transform.DOKill();
        item.transform.SetParent(null);

        return item;
    }

    public GameObject PeekItem()
    {
        if (IsEmpty) return null;
        return stackedItems[^1];
    }
}
