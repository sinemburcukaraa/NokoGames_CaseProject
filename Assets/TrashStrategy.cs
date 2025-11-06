using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TrashStrategy : MachineBase
{
    [Header("Trash Machine Settings")]
    [SerializeField] private float destroyDelay = 0.4f;
    [SerializeField] private ParticleSystem destroyEffect;
    [SerializeField] private AudioClip destroySound;
    [SerializeField] private bool usePooling = true;
    [SerializeField] private float jumpPower = 1f;

    private AudioSource audioSource;

    protected override void Awake()
    {
        base.Awake();
        audioSource = GetComponent<AudioSource>();
    }

    public override IEnumerator ProcessItem(GameObject inputObject)
    {
        if (inputObject == null)
            yield break;

        yield return new WaitForSeconds(destroyDelay);

        Vector3 destroyPos = processingPoint != null ? processingPoint.position : inputObject.transform.position;

        if (destroyEffect != null)
            Instantiate(destroyEffect, destroyPos, Quaternion.identity);

        if (audioSource != null && destroySound != null)
            audioSource.PlayOneShot(destroySound);

        ReturnToPoolIfPossible(inputObject);
        yield return null;
    }

    public IEnumerator ProcessStack(StackSystem stack)
    {
        if (stack == null || stack.IsEmpty)
            yield break;

        while (!stack.IsEmpty)
        {
            GameObject obj = stack.RemoveItem();
            if (obj == null) yield break;

            obj.transform.SetParent(null);
            obj.SetActive(true);

            // Jump animasyonu
            yield return obj.transform
                .DOJump(processingPoint.position, jumpPower, 1, jumpDuration)
                .SetEase(Ease.OutQuad)
                .WaitForCompletion();

            // Destroy efektini paralel başlat (beklemeden)
            StartCoroutine(ProcessItem(obj));

            // Obje arası çok kısa gecikme
            yield return new WaitForSeconds(0.1f);
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