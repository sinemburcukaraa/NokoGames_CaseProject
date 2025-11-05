using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEditor.Callbacks;
using UnityEngine;

public class StackSystem : MonoBehaviour
{
    public Transform stackParent;
    public float itemHeight = 0.3f;
    public float jumpPower = 0.3f;       // jump yüksekliği
    public float moveDuration = 0.25f;   // animasyon süresi
    public float swayAmount = 0.03f;     // hafif sway

    private List<GameObject> stackedItems = new List<GameObject>();
    private Queue<GameObject> pendingItems = new Queue<GameObject>();

    public void AddItem(GameObject item)
    {
        stackedItems.Add(item);

        item.transform.SetParent(stackParent);

        Vector3 basePos = Vector3.up * itemHeight * (stackedItems.Count - 1);
        item.transform.localRotation = Quaternion.identity;
        item.transform.localPosition = new Vector3(0, 0, 0); // animasyon başlangıcı

        StartCoroutine(MoveAndSway(item, basePos));
    }

    private IEnumerator MoveAndSway(GameObject item, Vector3 targetPos)
    {
        // Jump: sadece target Y üzerinden hareket
        Vector3 jumpPos = new Vector3(targetPos.x, targetPos.y + jumpPower, targetPos.z);
        // yield return item.transform.DOLocalMove(jumpPos, moveDuration).SetEase(Ease.OutQuad).WaitForCompletion();
        yield return item.transform.DOLocalJump(targetPos,.5f,1,1).SetEase(Ease.OutQuad).WaitForCompletion();

        // Hafif tek seferlik sway
        // float randomX = Random.Range(-swayAmount, swayAmount);
        // float randomZ = Random.Range(-swayAmount, swayAmount);
        // Vector3 swayTarget = targetPos + new Vector3(randomX, 0, randomZ);
        // yield return item.transform.DOLocalMove(swayTarget, moveDuration).SetEase(Ease.InOutSine).WaitForCompletion();

        // Son pozisyonu kesin olarak hedefe ayarla
        item.transform.localPosition = targetPos;
    }

    public GameObject RemoveItem()
    {
        if (stackedItems.Count == 0) return null;

        GameObject item = stackedItems[^1];
        stackedItems.RemoveAt(stackedItems.Count - 1);

        item.transform.DOKill();
        item.transform.SetParent(null);

        return item;
    }

    public int Count => stackedItems.Count;
}
