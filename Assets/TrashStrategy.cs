using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashStrategy : AssetProcessor
{
    public override void Interact(Transform interactor)
    {
        Object.Destroy(interactor);
    }

  
}
