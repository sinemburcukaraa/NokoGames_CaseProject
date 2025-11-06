using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class StackMovement : MonoBehaviour

{
  

    public Transform stackParent;
    public List<Transform> stackedItems = new List<Transform>();

    public float maxSwayDistance = 0.3f;
    public float swayDuration = 0.15f; // DOTween süre
    public Ease swayEase = Ease.OutSine;

    private Vector3 lastCharacterPosition;

    void Start()
    {
        if(stackParent != null)
            lastCharacterPosition = stackParent.position;
    }

    void Update()
    {
        if(stackParent == null || stackedItems.Count == 0) return;

        Vector3 movementDelta = stackParent.position - lastCharacterPosition;
        float speedFactor = movementDelta.magnitude / Time.deltaTime;

        for(int i = 0; i < stackedItems.Count; i++)
        {
            Transform item = stackedItems[i];
            float weight = ((float)i / (stackedItems.Count - 1));
            weight = Mathf.Pow(weight, 2); // üstteki daha çok sallansın

            Vector3 swayOffset = -movementDelta.normalized * maxSwayDistance * weight * Mathf.Clamp(speedFactor, 0, 2f);
            Vector3 targetPos = stackParent.position + Vector3.up * i * 0.3f + swayOffset;

            // DOTween ile hareket ettir
            item.DOKill();
            item.DOMove(targetPos, swayDuration).SetEase(swayEase);
        }

        lastCharacterPosition = stackParent.position;
    }

    public void AddItem(Transform newItem)
    {
        newItem.SetParent(stackParent);
        stackedItems.Add(newItem);
    }

    public void RemoveItem(Transform item)
    {
        if(stackedItems.Contains(item))
        {
            stackedItems.Remove(item);
            item.SetParent(null);
            item.DOKill();
        }
    }
}
