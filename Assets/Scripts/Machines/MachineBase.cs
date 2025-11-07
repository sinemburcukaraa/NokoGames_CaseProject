using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public abstract class MachineBase : MonoBehaviour, IMachine
{
    [Header("Machine Areas")]
    [SerializeField]
    protected AreaStorage inputArea;

    [SerializeField]
    protected AreaStorage outputArea;

    [SerializeField]
    protected Transform processingPoint;

    [Header("Animation Settings")]
    [SerializeField]
    protected float jumpHeight = 1f;

    [SerializeField]
    protected float jumpDuration = 0.5f;

    protected bool isProcessing;
    public bool IsBusy => isProcessing;
    public Animator MachineAnim;

    [Header("Effects")]
    [SerializeField]
    protected List<ParticleSystem> workingParticles = new List<ParticleSystem>();

    protected virtual void Awake()
    {
        if (outputArea != null)
            outputArea.OnObjectAdded += HandleAreaChange;

        if (inputArea != null)
            inputArea.OnObjectRemoved += HandleAreaChange;
    }

    protected virtual void OnDestroy()
    {
        if (outputArea != null)
            outputArea.OnObjectAdded -= HandleAreaChange;

        if (inputArea != null)
            inputArea.OnObjectRemoved -= HandleAreaChange;
    }

    protected void HandleAreaChange()
    {
        TryStartProcessing();
    }
    protected void PlayParticles()
    {
        foreach (var ps in workingParticles)
        {
            if (ps != null && !ps.isPlaying)
                ps.Play();
        }
    }

    protected void StopParticles()
    {
        foreach (var ps in workingParticles)
        {
            if (ps != null && ps.isPlaying)
                ps.Stop();
        }
    }
    protected virtual void TryStartProcessing()
    {
        if (isProcessing)
            return;
        if (outputArea == null || inputArea == null)
            return;

        if (outputArea.HasAnyObject && !inputArea.IsFull)
            StartCoroutine(ProcessLoop());
    }
    private IEnumerator ProcessLoop()
    {
        isProcessing = true;
        MachineAnim.speed = 1;
        PlayParticles();

        while (outputArea.HasAnyObject && !inputArea.IsFull)
        {
            GameObject obj = outputArea.TakeTopObject();
            if (obj == null)
                break;

            yield return MoveToProcessingPoint(obj);
            yield return ProcessItem(obj);
        }

        MachineAnim.speed = 0;
        isProcessing = false;
        StopParticles();
    }

    private IEnumerator MoveToProcessingPoint(GameObject obj)
    {
        obj.SetActive(true);

        yield return obj
            .transform.DOJump(processingPoint.position, jumpHeight, 1, jumpDuration)
            .SetEase(Ease.OutQuad)
            .WaitForCompletion();

        obj.SetActive(false);
    }

    public abstract IEnumerator ProcessItem(GameObject inputObject);

    public virtual void StartProcess() => TryStartProcessing();

    public virtual void StopProcess() => isProcessing = false;
}
