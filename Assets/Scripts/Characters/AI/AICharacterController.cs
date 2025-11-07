using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICharacterController : CharacterBase
{
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private StackSystem stackSystem;
    [SerializeField] private AreaStorage spawnerOutput;
    [SerializeField] private AreaStorage transformerInput;
    [SerializeField] private AreaStorage transformerOutput;
    [SerializeField] private AreaStorage trashArea;
    [SerializeField] private Animator AICharacterAnimator;
    [SerializeField] private int maxCarryCount = 5;

    private int currentCarryCount = 0;

    private enum WorkerState { Idle, Collect, Deliver }
    private WorkerState currentState = WorkerState.Idle;
    private AreaStorage currentTarget;

    private void Start() => StartCoroutine(WorkerLoop());

    private IEnumerator WorkerLoop()
    {
        while (true)
        {
            switch (currentState)
            {
                case WorkerState.Idle:
                    DecideNextTask();
                    break;

                case WorkerState.Collect:
                    yield return MoveTo(currentTarget.transform.position);
                    yield return StartCoroutine(CollectFromArea(currentTarget));
                    break;

                case WorkerState.Deliver:
                    yield return MoveTo(currentTarget.transform.position);
                    yield return DeliverToArea(currentTarget);
                    break;
            }
            yield return null;
        }
    }

    private void DecideNextTask()
    {
        if (!transformerOutput.IsEmpty)
        {
            currentTarget = transformerOutput;
            currentState = WorkerState.Collect;
        }
        else if (!spawnerOutput.IsEmpty && !transformerInput.IsFull)
        {
            currentTarget = spawnerOutput;
            currentState = WorkerState.Collect;
        }
        else
        {
            currentState = WorkerState.Idle;
        }
    }

    private IEnumerator MoveTo(Vector3 targetPos)
    {
        AICharacterAnimator.SetBool("isRunning", true);

        while (Vector3.Distance(transform.position, targetPos) > 0.1f)
        {
            Vector3 dir = (targetPos - transform.position).normalized;
            dir.y = 0;
            if (dir.sqrMagnitude > 0.001f)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), rotationSpeed * Time.deltaTime);

            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        AICharacterAnimator.SetBool("isRunning", false);
    }

    private IEnumerator CollectFromArea(AreaStorage area)
    {
        while (!stackSystem.IsFull && currentCarryCount < maxCarryCount && !area.IsEmpty)
        {
            GameObject obj = area.TakeTopObject();
            if (obj != null)
            {
                stackSystem.AddItem(obj);
                currentCarryCount++;
                yield return new WaitForSeconds(0.05f); 
            }
            else
            {
                yield return new WaitForSeconds(0.05f);
            }
        }

        if (area == spawnerOutput)
            currentTarget = transformerInput;
        else if (area == transformerOutput)
            currentTarget = trashArea;

        currentState = WorkerState.Deliver;
    }

    private IEnumerator DeliverToArea(AreaStorage area)
    {
        if (stackSystem.Count == 0)
        {
            currentCarryCount = 0;
            currentState = WorkerState.Idle;
            yield break;
        }

        if (area == trashArea)
        {
            var trashMachine = trashArea.GetComponentInParent<TrashStrategy>();
            if (trashMachine != null)
                yield return StartCoroutine(trashMachine.ProcessStack(stackSystem));

            currentCarryCount = 0;
            currentState = WorkerState.Idle;
            yield break;
        }

        while (stackSystem.Count > 0 && !area.IsFull)
        {
            GameObject obj = stackSystem.RemoveItem();
            if (obj != null)
            {
                area.AddObject(obj);
                currentCarryCount--;
                yield return new WaitForSeconds(0.05f);
            }
        }

        currentCarryCount = 0;
        currentState = WorkerState.Idle;
    }
}
