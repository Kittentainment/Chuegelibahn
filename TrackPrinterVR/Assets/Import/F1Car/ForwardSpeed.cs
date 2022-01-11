using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForwardSpeed : MonoBehaviour
{

    private int _trackPiecesInProximity = 0;

    public bool IsInTrackProximity => _trackPiecesInProximity > 0;

    [SerializeField] private int speedFactor = 500;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void FixedUpdate()
    {
        if (IsInTrackProximity)
        {
            this.GetComponent<Rigidbody>().AddForce(-(transform.right) * (Time.deltaTime * speedFactor), ForceMode.Force);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == SortingLayer.NameToID("Track"))
        {
            _trackPiecesInProximity++;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == SortingLayer.NameToID("Track"))
        {
            if (_trackPiecesInProximity <= 0)
            {
                throw new Exception("ForwardSpeed::OnTriggerExit - Somehow we removed more track pieces than we added. should never be minus 0;");
            }
            _trackPiecesInProximity--;
        }
    }
}
