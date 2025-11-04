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
// using UnityEngine;

// public class SimpleTPSCamera : MonoBehaviour
// {
//     [Header("Target Settings")]
//     public Transform target;           // Karakter transformu
//     public Vector3 offset = new Vector3(0, 5, -7); // Kamera yüksekliği ve uzaklığı
//     public float followSpeed = 10f;    // Smooth takip hızı

//     [Header("Rotation Settings")]
//     public float pitch = 20f;          // Kamera yukarı-aşağı açısı sabit
//     public float yaw = 0f;             // Kamera karakterin arkasında döner
//     public float rotationSmoothSpeed = 5f;

//     private Vector3 currentVelocity;

//     void LateUpdate()
//     {
//         if (target == null) return;

//         // Hedef pozisyon (karakter + offset)
//         Vector3 desiredPosition = target.position + Quaternion.Euler(0, yaw, 0) * offset;

//         // Smooth hareket
//         transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref currentVelocity, 1f / followSpeed);

//         // Kameranın bakacağı pozisyon
//         Vector3 lookTarget = target.position + Vector3.up * 1.5f; // karakterin üstüne bak
//         Quaternion desiredRotation = Quaternion.Euler(pitch, yaw, 0f);
//         transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, rotationSmoothSpeed * Time.deltaTime);

//         // Karaktere bakmasını sağla
//         transform.LookAt(lookTarget);
//     }
// }
