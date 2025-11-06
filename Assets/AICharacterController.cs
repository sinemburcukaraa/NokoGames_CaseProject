using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// [RequireComponent(typeof(StackSystem))]
public class AICharacterController : MonoBehaviour// CharacterBase, IWorker
{
    // [Header("Movement Settings")]
    // [SerializeField] private float moveSpeed = 3f;
    // [SerializeField] private float rotationSpeed = 5f;

    // [Header("Target Areas")]
    // [SerializeField] private List<Area> targetAreas;

    // private StackSystem stackSystem;
    // private Area currentTarget;
    // private bool isInteracting = false;

    // private void Awake()
    // {
    //     stackSystem = GetComponent<StackSystem>();
    // }

    // private void Update()
    // {
    //     if (isInteracting || targetAreas.Count == 0) return;

    //     if (currentTarget == null || !IsTargetValid(currentTarget))
    //         ChooseNextTarget();

    //     MoveTowardsTarget();
    // }

    // private bool IsTargetValid(Area area)
    // {
    //     return (area.IsInput && stackSystem.Count > 0) ||
    //            (area.IsOutput && area.HasAnyObject());
    // }

    // private void ChooseNextTarget()
    // {
    //     currentTarget = targetAreas[Random.Range(0, targetAreas.Count)];
    // }

    // private void MoveTowardsTarget()
    // {
    //     Vector3 direction = (currentTarget.transform.position - transform.position).normalized;
    //     transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), rotationSpeed * Time.deltaTime);
    //     transform.position += direction * moveSpeed * Time.deltaTime;

    //     if (Vector3.Distance(transform.position, currentTarget.transform.position) < 1f)
    //     {
    //         StartCoroutine(InteractWithArea(currentTarget));
    //     }
    // }

    // private IEnumerator InteractWithArea(Area area)
    // {
    //     isInteracting = true;

    //     area.Interact(transform); // AI stack ve area kontrolünü kendi içinde yapmalı

    //     yield return new WaitForSeconds(0.5f);

    //     isInteracting = false;
    // }
}
