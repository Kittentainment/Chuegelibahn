using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Copy
{
    [RequireComponent(typeof(XRGrabInteractable))]
    public class Copyable : MonoBehaviour
    {
        private const float RotationSpeedWhileTarget = 360f / 4;

        // /// <summary>
        // /// If true, the object will have a preview look. Currently not possible to switch back to the old material.
        // /// </summary>
        // [SerializeField] private bool usePreviewMaterial = true;
        
        public XRGrabInteractable grabInteractable { get; private set; }
        
        [CanBeNull] public CopyInput currentCopyInput { get; protected internal set; }

        [CanBeNull] private CopyOutputLocation _currentCopyOutputLocation;
        [CanBeNull]
        public CopyOutputLocation currentCopyOutputLocation
        {
            get => _currentCopyOutputLocation;
            protected internal set
            {
                if (value != null)
                    OnBecameCopyOutput();
                else
                    OnStoppedBeingCopyOutput();
                _currentCopyOutputLocation = value;
            }
        }
        private bool isInCopyInput => currentCopyInput != null;
        private bool isCopyOutput => currentCopyOutputLocation != null;
        

        private void Start()
        {
            grabInteractable = GetComponentInChildren<XRGrabInteractable>();
        }

        private void FixedUpdate()
        {
            if (isCopyOutput)
            {
                transform.Rotate(transform.up, Time.deltaTime * RotationSpeedWhileTarget);
            }
        }
        
        private void OnBecameCopyOutput()
        {
            // if (usePreviewMaterial)
            // {
            //     var previewMaterial = CreatePreviewMaterial();
            //     var children = GetComponentsInChildren<MeshRenderer>();
            //     for (var i = 0; i < children.Length; i++)
            //     {
            //         var meshRenderer = children[i];
            //         meshRenderer.material = previewMaterial;
            //     }
            // }
        }

        private void OnStoppedBeingCopyOutput()
        {
            
        }
        
        private Material CreatePreviewMaterial()
        {
            var previewMat = Resources.LoadAll("Materials", typeof(Material))
                .Cast<Material>()
                .First(mat => mat.name.Equals("CopyPreview"));
            return previewMat;
        }
        
        protected internal void LetGoWhileInCopyArea(SelectExitEventArgs args)
        {
            transform.SetPositionAndRotation(currentCopyInput!.transform.position, Quaternion.identity);
        }

        internal void GrabbedWhileInCopyArea(SelectEnterEventArgs args)
        {
            
        }
        

    }
}