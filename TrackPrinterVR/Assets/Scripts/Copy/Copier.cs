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

        private void CreateCopyPreview(Copyable copyable)
        {
            var copyGO = Instantiate(copyable.gameObject);
            copyGO.transform.position = _copyOutputLocation.transform.position;
            var copyableCopy = copyGO.GetComponent<Copyable>();
            copyableCopy.currentCopyOutputLocation = _copyOutputLocation;
            copyableCopy.grabInteractable.selectEntered.AddListener(OnCopyOutputGrabbed);
            _copyOutputLocation.AddNewCopyOutput(copyableCopy);
        }

        public void OnCopyableInputRemoved(Copyable copyable)
        {
            if (_copyOutputLocation.currentOutput != copyable)
            {
                Debug.LogWarning("Copier::OnCopyableRemoved the removed object was not stored");
            }
            _copyOutputLocation.RemoveCopyOutput(false);
        }

        private void OnCopyOutputGrabbed(SelectEnterEventArgs args)
        {
            Debug.Log("OnCopyOutputGrabbed");
            var copyable = args.interactableObject.transform.GetComponent<Copyable>();
            copyable.grabInteractable.selectEntered.RemoveListener(OnCopyOutputGrabbed);
            copyable.currentCopyOutputLocation = null;
            CreateCopyPreview(_copyInput.currentCopyObject);
        }
    }
}