using UnityEngine;

public class StackMovement : MonoBehaviour
{
    [Header("References")]
    public Transform stackParent;
    public StackSystem stackSystem;

    [Header("Stack Settings")]
    public float verticalSpacing = 0.3f;

    [Header("Physics Sway Settings")]
    public float stiffness = 15f;
    public float damping = 5f;
    public float chainInfluence = 0.35f;

    [Header("Rotation Sway")]
    public float maxYawAngle = 25f;
    public float maxTiltX = 15f;
    public float maxRoll = 8f;
    public float swaySmooth = 6f;
    public float oscilAmplitude = 5f;

    [Header("Reverse Reaction Settings")]
    public float maxReverseFactor = 0.5f; // maksimum geri tepme

    // Internal
    private Vector3 lastParentPos;
    private Vector3[] offsets;
    private float[] sideSwayOffsets;    // Yaw
    private float[] forwardSwayOffsets; // Pitch
    private float[] rollOffsets;        // Z tilt
    private float[] velocitiesX;
    private float[] velocitiesZ;

    void Start()
    {
        if (stackParent == null || stackSystem == null) return;
        EnsureArrays();
        lastParentPos = stackParent.position;
    }

    void EnsureArrays()
    {
        int count = Mathf.Max(1, stackSystem.stackedItems.Count);
        if (offsets == null || offsets.Length != count)
        {
            offsets = new Vector3[count];
            velocitiesX = new float[count];
            velocitiesZ = new float[count];
            sideSwayOffsets = new float[count];
            forwardSwayOffsets = new float[count];
            rollOffsets = new float[count];

            for (int i = 0; i < count; i++)
            {
                offsets[i] = Vector3.zero;
                velocitiesX[i] = velocitiesZ[i] = 0f;
                sideSwayOffsets[i] = forwardSwayOffsets[i] = rollOffsets[i] = 0f;
            }
        }
    }

    void LateUpdate()
    {
        if (stackParent == null || stackSystem == null || stackSystem.stackedItems.Count == 0) return;
        EnsureArrays();

        Vector3 deltaPos = stackParent.position - lastParentPos;
        float speed = deltaPos.magnitude / Mathf.Max(Time.deltaTime, 0.0001f); // karakter hızı
        Vector3 moveDir = deltaPos.normalized; // hareket yönü
        int count = stackSystem.stackedItems.Count;
        int denom = Mathf.Max(count - 1, 1);

        bool isMoving = deltaPos.magnitude > 0.001f;

        // Dinamik geri tepme faktörü hızla çarpılıyor
        float dynamicReverse = Mathf.Clamp01(speed) * maxReverseFactor;

        for (int i = 0; i < count; i++)
        {
            Transform item = stackSystem.stackedItems[i].transform;
            if (item == null) continue;

            float normalizedIndex = (float)i / denom;
            float looseness = Mathf.Lerp(0.2f, 1f, normalizedIndex);

            Vector3 basePos = stackParent.position + Vector3.up * (i * verticalSpacing);

            // --- X ve Z ekseni: yaylanma + geri tepme
            Vector3 targetOffset = Vector3.zero;

            if (isMoving)
            {
                // mevcut osilatör sağa-sola ve hafif doğal salınım
                targetOffset.x = Mathf.Sin(Time.time * 5f + i * 0.5f) * oscilAmplitude * looseness;
                // geri tepme ters yönde deltaPos
                targetOffset.x += -moveDir.x * looseness * dynamicReverse;
                targetOffset.z = -moveDir.z * looseness * dynamicReverse; // ileri/geri tepki
            }

            // Zincir etkisi
            if (i > 0) targetOffset += offsets[i - 1] * chainInfluence;

            // X ve Z fizik hesaplama
            float forceX = (targetOffset.x - offsets[i].x) * stiffness - velocitiesX[i] * damping;
            velocitiesX[i] += forceX * Time.deltaTime;
            offsets[i].x += velocitiesX[i] * Time.deltaTime;
            sideSwayOffsets[i] = Mathf.Lerp(sideSwayOffsets[i], offsets[i].x, Time.deltaTime * swaySmooth);

            float forceZ = (targetOffset.z - offsets[i].z) * stiffness - velocitiesZ[i] * damping;
            velocitiesZ[i] += forceZ * Time.deltaTime;
            offsets[i].z += velocitiesZ[i] * Time.deltaTime;
            forwardSwayOffsets[i] = Mathf.Lerp(forwardSwayOffsets[i], offsets[i].z, Time.deltaTime * swaySmooth);

            // Roll efekti
            float targetRoll = offsets[i].x * 0.3f;
            if (i > 0) targetRoll += rollOffsets[i - 1] * 0.2f;
            rollOffsets[i] = Mathf.Lerp(rollOffsets[i], targetRoll, Time.deltaTime * swaySmooth);

            // Final pozisyon
            Vector3 finalPos = Vector3.Lerp(item.position, basePos + offsets[i], Time.deltaTime * swaySmooth);
            item.position = finalPos;

            // Rotasyon
            Transform mesh = item.childCount > 0 ? item.GetChild(0) : item;
            Quaternion targetRot = stackParent.rotation
                                    * Quaternion.Euler(forwardSwayOffsets[i], sideSwayOffsets[i], rollOffsets[i]);
            mesh.rotation = Quaternion.Slerp(mesh.rotation, targetRot, Time.deltaTime * swaySmooth);
        }

        lastParentPos = stackParent.position;
    }
}
