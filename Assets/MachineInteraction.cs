using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(AreaStorage))]
public class MachineInteraction : MonoBehaviour
{
    private AreaStorage storage;
    public event System.Action<GameObject> OnObjectTakenByMachine;

    private void Awake() => storage = GetComponent<AreaStorage>();

    public void MachineTakeObject()
    {
        if (!storage.HasAnyObject) return;

        GameObject obj = storage.TakeTopObject();
        if (obj != null)
            OnObjectTakenByMachine?.Invoke(obj);
    }
}
