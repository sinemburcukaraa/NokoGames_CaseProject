using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Follow Settings")]
    public Transform target;
    public Vector3 offset = new Vector3(0, 5, -7);
    public float followSpeed = 5f;
    public float rotateSpeed = 3f;

    private float yaw = 0f;

    void FixedUpdate()
    {
        if (target == null)
            return;

        if (Input.touchCount > 0)
        {
            foreach (Touch touch in Input.touches)
            {
                if (touch.position.x > Screen.width / 2f)
                {
                    if (touch.phase == TouchPhase.Moved)
                    {
                        yaw += touch.deltaPosition.x * rotateSpeed * 0.01f;
                    }
                }
            }
        }

        Vector3 desiredPosition = target.position + Quaternion.Euler(0, yaw, 0) * offset;
        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            followSpeed * Time.deltaTime
        );

        Vector3 lookPos = target.position + Vector3.up * 1.5f;
        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            Quaternion.LookRotation(lookPos - transform.position),
            followSpeed * Time.deltaTime
        );
    }
}
