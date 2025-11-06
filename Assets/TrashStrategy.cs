using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashStrategy : MonoBehaviour, IInteractable
{
    public void Interact(Transform interactor)
    {
        Object.Destroy(interactor);
    }


}
