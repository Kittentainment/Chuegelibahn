using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableInTrash : MonoBehaviour
{
    public bool IsInTrashCan { get; private set; }
    private Rigidbody rigi;
    private int sentinel = 0;

    private void Awake()
    {
        rigi = GetComponent<Rigidbody>();
    }

    public void OnLetGo()
    {
        Debug.Log("Let go of Object, isInTrash is: " + IsInTrashCan + "  Name of object: " + rigi.gameObject.name);
        if (!IsInTrashCan)
        {
            // rigi.isKinematic = true;
        }
        else
        {
            rigi.isKinematic = false;
            rigi.useGravity = true;
            Debug.Log("throwaway item is now: \nisKinematic: " + rigi.isKinematic + "\nuseGravity: " + rigi.useGravity);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (sentinel == 0)
        {

            if (other.CompareTag("Trashcan"))
            {
                Debug.Log("Set is in Trashcan to True");
                IsInTrashCan = true;
                sentinel++;
            }
            
        }
        if (other.CompareTag("TrashFire"))
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("LeaveTrashcan"))
        {
            Debug.Log("Hit the floor, make kinematic");

            rigi.isKinematic = true;
        }
        
        if (sentinel != 0)
        {
            if (other.CompareTag("Trashcan"))
            {

                IsInTrashCan = false;
                sentinel = 0;
                
            }
            
            
            
        }
    }
}