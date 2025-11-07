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

    [SerializeField] private int maxCapacity = 10;
    public List<GameObject> stackedItems = new List<GameObject>();

    public bool isProcessing = false;
    public int Count => stackedItems.Count;
    public bool IsFull => stackedItems.Count >= maxCapacity;
    public bool IsEmpty => stackedItems.Count == 0;

    private Dictionary<GameObject, Tween> activeTweens = new Dictionary<GameObject, Tween>();

    public void AddItem(GameObject item)
    {
        if (IsFull) return;

        stackedItems.Add(item);
        item.transform.SetParent(stackParent);
        item.transform.rotation = Quaternion.identity;

        StartCoroutine(AnimateItem(item));
    }

    private IEnumerator AnimateItem(GameObject item)
    {
        {
            int index = stackedItems.IndexOf(item);
            if (index < 0) yield break;

            isProcessing = true;

            Vector3 targetPos = Vector3.up * itemHeight * index;
            Tween jumpTween = item.transform.DOLocalJump(targetPos, jumpPower, 1, moveDuration).SetEase(Ease.OutQuad);
            activeTweens[item] = jumpTween;

            yield return new WaitForSeconds(0.05f);
            activeTweens.Remove(item);

            isProcessing = false;
        }
    }

    public IEnumerator CompleteActiveAnimations(float delay = 0.05f)
    {

        foreach (var kvp in new Dictionary<GameObject, Tween>(activeTweens))
        {
            yield return new WaitForSeconds(delay);
        }
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

    public GameObject PeekItem() => IsEmpty ? null : stackedItems[^1];
}
