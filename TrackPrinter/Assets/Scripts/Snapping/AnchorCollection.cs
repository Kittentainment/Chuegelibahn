using System.Collections.Generic;
using UnityEngine;

namespace Snapping
{
    public class AnchorCollection : MonoBehaviour
    {
        public void AddAnchorsAsChildren(List<Anchor> anchors)
        {
            anchors.ForEach(anchor => anchor.gameObject.transform.parent = this.transform);
        }
    }
}