using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Copy
{
    public class CopyInput : MonoBehaviour
    {
        [CanBeNull] public Copyable currentCopyObject { get; private set; }
        public Copier copier { get; private set; }


        private void Start()
        {
            copier = GetComponentInParent<Copier>();
        }

        private void OnTriggerEnter(Collider other)
        {
            var copyable = other.GetComponentInParent<Copyable>();
            if (copyable == null) return;

            Debug.Log("Printer Input Area Entered");
            if (copyable.grabInteractable.isSelected)
            {
                copyable.grabInteractable.selectExited.AddListener(LetGoWhileInCopyArea);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            var copyable = other.GetComponentInParent<Copyable>();
            if (copyable == null) return;
            
            Debug.Log("Printer Input Area Left");

            copyable.grabInteractable.selectExited.RemoveListener(LetGoWhileInCopyArea);
        }

        private void LetGoWhileInCopyArea(SelectExitEventArgs args)
        {
            if (currentCopyObject != null)
            {
                Debug.Log("Added a second Copyable into the CopyInput. Ignored.");
                return;
            }
            
            var copyable = args.interactableObject.transform.gameObject.GetComponent<Copyable>();
            
            currentCopyObject = copyable;
            copyable.currentCopyInput = this;
            copyable.LetGoWhileInCopyArea(args);
            args.interactableObject.selectEntered.AddListener(GrabbedWhileInCopyArea);

            copier.OnCopyableAdded(copyable);
        }

        private void GrabbedWhileInCopyArea(SelectEnterEventArgs args)
        {
            var copyable = args.interactableObject.transform.gameObject.GetComponent<Copyable>();
            if (copyable != currentCopyObject)
            {
                Debug.LogWarning(
                    "Somehow another Copyable was removed from the CopyInput than was stored in the CopyInput. Ignored.");
                return;
            }

            currentCopyObject = null;
            copyable.currentCopyInput = null;
            copyable.GrabbedWhileInCopyArea(args);
            args.interactableObject.selectEntered.RemoveListener(GrabbedWhileInCopyArea);
            
            copier.OnCopyableRemoved(copyable);
        }
    }
}