using System.Linq;
using UnityEngine;

namespace Snapping
{
    public class ObjToSnap : MonoBehaviour
    {
        private Material CreatePreviewMaterial()
        {
            var previewMat = Resources.LoadAll("Materials", typeof(Material))
                .Cast<Material>()
                .First(mat => mat.name.Equals("SnappingPreview"));
            return previewMat;
        }

        /// <summary>
        /// Returns an object to represent the position, where this object will snap to, if let go.
        /// This method can be changed and overwritten to calculate the object or clone it from a prefab.
        /// The object should preferably be somehow see-through.
        /// It will be destroyed after it's not needed anymore (snapping radius left, or let got), therefore if another
        /// object is used as a base, it has to be cloned (for example with Instantiate).
        /// </summary>
        public GameObject CreateSnappingPreviewObject(Transform parentTransform)
        {
            var previewGO = Instantiate(gameObject, parentTransform, true);
            previewGO.GetComponent<ObjToSnap>().enabled = false;
            var previewMaterial = CreatePreviewMaterial();
            foreach (var meshRenderer in previewGO.GetComponentsInChildren<MeshRenderer>())
            {
                meshRenderer.material = previewMaterial;
            }

            return previewGO;
        }
    }
}