using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEditor.Callbacks;
using UnityEngine;

public class StackSystem : MonoBehaviour
{
    public Transform stackParent;
    public float itemHeight = 0.3f;
    public float jumpPower = 0.3f; // jump yüksekliği
    public float moveDuration = 0.25f; // animasyon süresi
    public float swayAmount = 0.03f; // hafif sway

    private List<GameObject> stackedItems = new List<GameObject>();
    private Queue<GameObject> pendingItems = new Queue<GameObject>();

    public void AddItem(GameObject item)
    {
        stackedItems.Add(item);
        item.transform.SetParent(stackParent);

        Vector3 targetPos = Vector3.up * itemHeight * (stackedItems.Count - 1);
        item.transform.localRotation = Quaternion.identity;
        item.transform.DOKill();

        // Animasyonla stacke yerleştir
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
        if (stackedItems.Count == 0) return null;

        GameObject item = stackedItems[^1];
        stackedItems.RemoveAt(stackedItems.Count - 1);

        // Coroutine varsa durdur
        cube cubeComp = item.GetComponent<cube>();
        // if (cubeComp != null)
        //     cubeComp.StopFollowing();

        item.transform.DOKill();
        item.transform.SetParent(null);

        return item;
    }

    public int Count => stackedItems.Count;
}
