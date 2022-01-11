using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableInTrash : MonoBehaviour
{
    public bool IsInTrashCan { get; private set; }

    public void OnLetGo()
    {
        if (!IsInTrashCan) return;
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Rigidbody>().useGravity = true;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Trashcan"))
        {
            IsInTrashCan = true;
        }

        if (other.CompareTag("TrashFire"))
        {
            Destroy(gameObject);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Trashcan"))
        {
            IsInTrashCan = false;
        }
    }
}
