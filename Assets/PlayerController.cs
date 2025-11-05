using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;

    [Header("Joystick")]
    public FloatingJoystick joystick;

    private Rigidbody rb;
    private Animator animator;
    private Vector3 moveDirection;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        animator = GetComponent<Animator>();
    }

    void Update()
    {
        float h = joystick.Horizontal;
        float v = joystick.Vertical;

        moveDirection = new Vector3(h, 0, v).normalized;

        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        bool isMoving = moveDirection.magnitude > 0.1f;
        animator.SetBool("isRunning", isMoving);
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime);
    }

    // public override void MoveToTarget(Vector3 position)
    // {
    //     Vector3 direction = (position - transform.position).normalized;
    //     rb.MovePosition(
    //         Vector3.MoveTowards(rb.position, position, moveSpeed * Time.fixedDeltaTime)
    //     );

    //     if (direction != Vector3.zero)
    //     {
    //         Quaternion targetRotation = Quaternion.LookRotation(direction);
    //         transform.rotation = Quaternion.Lerp(
    //             transform.rotation,
    //             targetRotation,
    //             rotationSpeed * Time.fixedDeltaTime
    //         );
    //     }

    //     bool isMoving = Vector3.Distance(transform.position, position) > 0.05f;
    //     animator.SetBool("isRunning", isMoving);
    // }
}

