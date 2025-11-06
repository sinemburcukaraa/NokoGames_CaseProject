using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent<IInteractable>(out var interactable))
        {
            interactable.Interact(transform);
            print("interact ");
            // GameEvents.Instance.TriggerInteract(transform, interactable);
        }
    }
}
