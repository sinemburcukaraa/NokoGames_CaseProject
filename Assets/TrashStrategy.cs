using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashStrategy : MachineBase
{
    [Header("Trash Machine Settings")]
    [SerializeField] private float destroyDelay = 0.4f;
    [SerializeField] private ParticleSystem destroyEffect;
    [SerializeField] private AudioClip destroySound;
    [SerializeField] private bool usePooling = true;

    private AudioSource audioSource;

    protected override void Awake()
    {
        base.Awake();
        audioSource = GetComponent<AudioSource>();
    }

    protected override IEnumerator ProcessItem(GameObject inputObject)
    {
        if (inputObject == null)
            yield break;

        // Küçük bir gecikme (yok etme animasyonu/efekti için)
        yield return new WaitForSeconds(destroyDelay);

        Vector3 destroyPos = processingPoint != null ? processingPoint.position : inputObject.transform.position;

        // 🎇 Efekt ve ses
        if (destroyEffect != null)
            Instantiate(destroyEffect, destroyPos, Quaternion.identity);

        if (audioSource != null && destroySound != null)
            audioSource.PlayOneShot(destroySound);

        // ♻️ Pool veya Destroy işlemi
        ReturnToPoolIfPossible(inputObject);

        yield return null;
    }

    private void ReturnToPoolIfPossible(GameObject obj)
    {
        // Objede bir ResourceItem var mı?
        var resourceItem = obj.GetComponent<ResourceItem>();
        if (resourceItem != null && usePooling)
        {
            // Eğer bu obje spawner tarafından oluşturulduysa, genellikle
            // onun içinde bir ObjectPool referansı tutulur.
            if (resourceItem.originPool != null)
            {
                // Pool’a geri gönder
                resourceItem.originPool.ReturnToPool(resourceItem.transform);
                return;
            }
        }

        // Eğer hiçbir pool referansı yoksa, normal şekilde yok et
        Destroy(obj);
    }
}