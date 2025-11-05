using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEditor.Callbacks;
using UnityEngine;

public class StackSystem : MonoBehaviour
{
    public Transform stackParent;
    public float itemHeight = 0.3f;
    public float swayAmount = 0.5f;
    public float swayDuration = 0.2f;

    private List<GameObject> stackedItems = new List<GameObject>();
    private int _cubeListIndexCounter = 0;
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Product"))
        {
            AddItem(collision.gameObject);
        }
    }

    public void AddItem(GameObject item)
    {
        stackedItems.Add(item);

        // Parent olarak stackParent'ı kullanıyoruz
        item.transform.SetParent(stackParent);

        // localPosition kullanıyoruz, ama stackParent world position ile hizalanmış
        Vector3 newPos = Vector3.up * itemHeight * (stackedItems.Count - 1);
        item.transform.localPosition = newPos;

        // Rotasyonu sıfırla
        item.transform.localRotation = Quaternion.identity;
        // if (stackedItems.Count == 1)
        // {
        //     item.GetComponent<cube>().UpdateCubePosition(this.transform, true);
        // }
        // else
        // {
        //     item.GetComponent<cube>().UpdateCubePosition(stackedItems[_cubeListIndexCounter].transform, true);
        //     _cubeListIndexCounter++;
        // }
    }
    public GameObject RemoveItem()
    {
        if (stackedItems.Count == 0) return null;

        GameObject item = stackedItems[^1];
        stackedItems.RemoveAt(stackedItems.Count - 1);
        return item;
    }

    public int Count => stackedItems.Count;
}
