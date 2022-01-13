using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ForwardSpeed : MonoBehaviour
{
    private int _trackLayerName;

    private int _trackPiecesInProximity = 0;

    public bool IsInTrackProximity => _trackPiecesInProximity > 0;

    [SerializeField] private int speedFactor = 500;

    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = this.GetComponent<Rigidbody>();
        _trackLayerName = SortingLayer.NameToID("Track");
    }

    private void FixedUpdate()
    {
        if (IsInTrackProximity)
        {
            _rigidbody.AddForce(-(transform.right) * (Time.deltaTime * speedFactor), ForceMode.Force);
        }
    }
    
    private void OnCollisionEnter(Collision other)
    {
        Debug.Log("Car OnTriggerEnter");
        if (other.gameObject.layer == _trackLayerName)
        {
            _trackPiecesInProximity++;
        }
    
        if (_trackPiecesInProximity == 1)
        {
            _rigidbody.isKinematic = false;
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.layer == _trackLayerName)
        {
            if (_trackPiecesInProximity <= 0)
            {
                throw new Exception("ForwardSpeed::OnTriggerExit - Somehow we removed more track pieces than we added. should never be minus 0;");
            }
            _trackPiecesInProximity--;
        }

        if (_trackPiecesInProximity <= 0)
        {
            _rigidbody.isKinematic = true;
        }
    }
}
