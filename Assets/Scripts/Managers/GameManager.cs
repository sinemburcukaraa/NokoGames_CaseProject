using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject objectToSpawn;
    public Button hireButton; 

    public void SpawnWorker()
    {
        if (objectToSpawn == null ) return;

        objectToSpawn.SetActive(true);

        if (hireButton != null)
            hireButton.interactable = false;

        if (UIManager.Instance != null)
            UIManager.Instance.ShowNotification("Worker hired!");
    }
}
