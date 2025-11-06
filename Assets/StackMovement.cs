using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class StackMovement : MonoBehaviour
{
    public Transform stackParent;
    public float maxSwayDistance = 0.3f;
    public float swayDuration = 0.15f;
    public Ease swayEase = Ease.OutSine;

    private Vector3 lastCharacterPosition;
    public StackSystem stackSystem;

    void Start()
    {
        if (stackParent != null)
            lastCharacterPosition = stackParent.position;
    }

    void Update()
    {
        if (stackParent == null || stackSystem.Count == 0) return;

        Vector3 movementDelta = stackParent.position - lastCharacterPosition;
        float speedFactor = movementDelta.magnitude / Time.deltaTime;

        for (int i = 0; i < stackSystem.stackedItems.Count; i++)
        {
            Transform item = stackSystem.stackedItems[i].transform;
            float weight = ((float)i / (stackSystem.stackedItems.Count - 1));
            weight = Mathf.Pow(weight, 2); // üstteki daha çok sallansın

            Vector3 swayOffset = -movementDelta.normalized * maxSwayDistance * weight * Mathf.Clamp(speedFactor, 0, 2f);
            Vector3 targetPos = stackParent.position + Vector3.up * i * 0.3f + swayOffset;

            // DOTween ile hareket ettir
            item.DOKill();
            item.DOMove(targetPos, swayDuration).SetEase(swayEase);
        }

        lastCharacterPosition = stackParent.position;
    }
}
