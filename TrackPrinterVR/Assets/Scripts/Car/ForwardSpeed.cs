using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Car
{
    [RequireComponent(typeof(Rigidbody))]
    public class ForwardSpeed : MonoBehaviour
    {
        private int _trackLayer;

        private int _trackPiecesInProximity = 0;

        public bool IsInTrackProximity => _trackPiecesInProximity > 0;

        [SerializeField] private int speedFactor = 500;

        private Rigidbody _rigidbody;
        private XRGrabInteractable _grabInteractable;

        private void Awake()
        {
            _rigidbody = this.GetComponent<Rigidbody>();
            // var collider = GetComponent<BoxCollider>();
        
            _grabInteractable = GetComponent<XRGrabInteractable>();
            _grabInteractable.selectExited.AddListener(args =>
            {
            
            });
            _trackLayer = 6; // SortingLayer.NameToID("Track");
        }

        private void FixedUpdate()
        {
            if (IsInTrackProximity)
            {
                _rigidbody.AddForce(-(transform.right) * (Time.deltaTime * speedFactor), ForceMode.Force);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            RegisterTrackPiece(other.gameObject);
        }

        private void OnTriggerExit(Collider other)
        {
            DeregisterTrackPiece(other.gameObject);
        }

        private void OnCollisionEnter(Collision other)
        {
            RegisterTrackPiece(other.gameObject);
        }

        private void OnCollisionExit(Collision other)
        {
            DeregisterTrackPiece(other.gameObject);
        }

        private void RegisterTrackPiece(GameObject other)
        {
            if (other.layer != _trackLayer)
            {
               // Debug.Log("collision with others");
                return;
                
            }

           // Debug.Log("Registered TRACK collision");
            
            _trackPiecesInProximity++;
            if (_trackPiecesInProximity >= 1)
            {
                _rigidbody.isKinematic = false;
            }


        }

        private void DeregisterTrackPiece(GameObject other)
        {
            if (other.layer != _trackLayer) return;
            
            if (_trackPiecesInProximity <= 0)
            {
                throw new Exception(
                    "ForwardSpeed::OnTriggerExit - Somehow we removed more track pieces than we added. should never be minus 0;");
            }
            _trackPiecesInProximity--;
            if (_trackPiecesInProximity <= 0)
            {
                _rigidbody.isKinematic = true;
            }

        }
    }
}