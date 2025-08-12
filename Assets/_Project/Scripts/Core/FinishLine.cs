using UnityEngine;
using System;


public class FinishLine : MonoBehaviour
{
    public static event Action OnPlayerCrossedFinish;

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Player"))
        {
            Debug.Log("Игрок пересек финишную черту!");


            OnPlayerCrossedFinish?.Invoke();
        }
    }
}
