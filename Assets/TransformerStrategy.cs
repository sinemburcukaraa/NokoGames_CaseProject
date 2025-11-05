using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformerStrategy : AssetProcessor
{
    public override void Interact(Transform interactor)
    {
        GameObject output = Object.Instantiate(Resources.Load<GameObject>("ProcessedItem"));
        output.transform.position = interactor.position;
        Object.Destroy(interactor);
    }
}
