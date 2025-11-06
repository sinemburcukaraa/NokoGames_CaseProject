using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class StackSystem : MonoBehaviour
{
    [Header("Stack Settings")]
    public Transform stackParent;
    public float itemHeight = 0.3f;
    public float jumpPower = 0.3f; // jump yüksekliği
    public float moveDuration = 0.25f; // animasyon süresi
    public float swayAmount = 0.03f; // hafif sway
    [SerializeField] private int maxCapacity = 10; // 🆕 maksimum taşıma limiti

    public List<GameObject> stackedItems = new List<GameObject>();

    public int Count => stackedItems.Count;
    public bool IsFull => stackedItems.Count >= maxCapacity;
    public bool IsEmpty => stackedItems.Count == 0;

    public void AddItem(GameObject item)
    {
        if (IsFull) return;

        stackedItems.Add(item);
        item.transform.SetParent(stackParent);

        Vector3 targetPos = Vector3.up * itemHeight * (stackedItems.Count - 1);
        item.transform.localRotation = Quaternion.identity;
        item.transform.DOKill();

        StartCoroutine(AnimateItem(item, targetPos));
    }

    private IEnumerator AnimateItem(GameObject item, Vector3 targetPos)
    {
        yield return item.transform.DOLocalJump(targetPos, jumpPower, 1, moveDuration)
            .SetEase(Ease.OutQuad)
            .WaitForCompletion();

        item.transform.localPosition = targetPos;
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
