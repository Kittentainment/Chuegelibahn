using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForwardSpeed : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void FixedUpdate()
    {
        this.GetComponent<Rigidbody>().AddForce(-(transform.right) * (Time.deltaTime * 750), ForceMode.Force);
    }
}// 
