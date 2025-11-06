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
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        HandleInput();
        HandleRotation();
        UpdateAnimation();
    }

    private void FixedUpdate()
    {
        Move();
    }

    #region Movement
    private void HandleInput()
    {
        moveDirection = new Vector3(joystick.Horizontal, 0, joystick.Vertical).normalized;
    }

    private void HandleRotation()
    {
        if (moveDirection == Vector3.zero) return;

        Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private void Move()
    {
        if (moveDirection == Vector3.zero) return;
        rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime);
    }

    private void UpdateAnimation()
    {
        bool isMoving = moveDirection.magnitude > 0.1f;
        animator.SetBool("isRunning", isMoving);
    }
    #endregion
}

