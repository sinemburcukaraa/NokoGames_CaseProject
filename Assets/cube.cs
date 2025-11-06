using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class cube : MonoBehaviour
{
     [SerializeField] private float followSpeed = 10f;
    private Coroutine followRoutine;

    public void UpdateCubePosition(Transform followedCube, bool isFollowStart)
    {
        // Eğer önceki Coroutine varsa durdur
        if (followRoutine != null)
            StopCoroutine(followRoutine);

        followRoutine = StartCoroutine(FollowXZ(followedCube, isFollowStart));
    }

    private IEnumerator FollowXZ(Transform followedCube, bool isFollow)
    {
        while (isFollow)
        {
            yield return null;
            // Y eksenini sabitle, sadece XZ takip et
            transform.position = new Vector3(
                Mathf.Lerp(transform.position.x, followedCube.position.x, followSpeed * Time.deltaTime),
                transform.position.y, // Y DOTween tarafından kontrol edilecek
                Mathf.Lerp(transform.position.z, followedCube.position.z, followSpeed * Time.deltaTime)
            );
        }
    }

    public void StopFollowing()
    {
        if (followRoutine != null)
            StopCoroutine(followRoutine);
    }
}
