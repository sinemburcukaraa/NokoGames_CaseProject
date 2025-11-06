using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
     public static GameEvents Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    public event Action<Transform, IInteractable> OnInteract;

    public void TriggerInteract(Transform interactor, IInteractable interactable)
    {
        OnInteract?.Invoke(interactor, interactable);
    }
}
