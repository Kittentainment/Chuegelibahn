using Snapping;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Copy
{
    public class Copier : MonoBehaviour
    {
        private CopyInput _copyInput;
        private CopyOutputLocation _copyOutputLocation;

        private void Start()
        {
            _copyInput = GetComponentInChildren<CopyInput>();
            _copyOutputLocation = GetComponentInChildren<CopyOutputLocation>();
        }
        

        public void OnCopyableAdded(Copyable copyable)
        {
            CreateCopyPreview(copyable);
        }

        private void CreateCopyPreview(Copyable copyableOriginal)
        {
            var copyGO = Instantiate(copyableOriginal.gameObject);
            copyGO.transform.position = _copyOutputLocation.transform.position;
            var copyableCopy = copyGO.GetComponent<Copyable>();
            copyableCopy.currentCopyOutputLocation = _copyOutputLocation;
            copyableCopy.grabInteractable.selectEntered.AddListener(OnCopyOutputGrabbed);
            HandleSpecialCopyCases(copyableCopy);
            _copyOutputLocation.AddNewCopyOutput(copyableCopy);
        }

        /// <summary>
        /// Some kind of objects need additional code for copying them. For example Listeners are not copied on Instantiate().
        /// </summary>
        /// <param name="copyable">The Copyable which has just been copied via Instantiate() and might need special handling</param>
        private void HandleSpecialCopyCases(Copyable copyable)
        {
            var snappingObjWrapper = copyable.GetComponent<SnappingObjWrapper>();
            if (snappingObjWrapper != null)
            {
                snappingObjWrapper.AddGrabListeners();
            }
        }

        public void OnCopyableInputRemoved(Copyable copyable)
        {
            if (_copyOutputLocation.currentOutput != copyable)
            {
                Debug.LogWarning("Copier::OnCopyableRemoved the removed object was not stored");
            }
            _copyOutputLocation.RemoveCopyOutput(true);
        }

        private void OnCopyOutputGrabbed(SelectEnterEventArgs args)
        {
            Debug.Log("OnCopyOutputGrabbed");
            _copyOutputLocation.RemoveCopyOutput(false);
            var copyable = args.interactableObject.transform.GetComponent<Copyable>();
            copyable.grabInteractable.selectEntered.RemoveListener(OnCopyOutputGrabbed);
            copyable.currentCopyOutputLocation = null;
            CreateCopyPreview(_copyInput.currentCopyObject);
        }
    }
}