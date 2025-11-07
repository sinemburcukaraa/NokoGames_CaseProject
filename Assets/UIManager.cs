using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public TMP_Text notificationText;
    public float displayTime = 2f;

    private Coroutine currentCoroutine;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        notificationText.text = "";
        notificationText.alpha = 0f;
    }

    public void ShowNotification(string message)
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
        currentCoroutine = StartCoroutine(DisplayNotification(message));
    }

    private IEnumerator DisplayNotification(string message)
    {
        notificationText.text = message;

        float fadeDuration = 0.3f;
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            notificationText.alpha = t / fadeDuration;
            yield return null;
        }
        notificationText.alpha = 1f;

        yield return new WaitForSeconds(displayTime);

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            notificationText.alpha = 1 - (t / fadeDuration);
            yield return null;
        }
        notificationText.alpha = 0f;
        notificationText.text = "";
    }
}
