using JetBrains.Annotations;
using UnityEngine;

namespace Copy
{
    public class CopyOutputLocation : MonoBehaviour
    {
        [CanBeNull]
        public Copyable currentOutput { get; private set; }

        /// <summary>
        /// Add a new copy output and destroy a new one if it exists.
        /// </summary>
        /// <param name="newOutput">The new copy output</param>
        public void AddNewCopyOutput([NotNull] Copyable newOutput)
        {
            if (currentOutput != null)
            {
                Destroy(currentOutput.gameObject);
            }
            currentOutput = newOutput;
        }

        /// <summary>
        /// Remove current copy output.
        /// </summary>
        public void RemoveCopyOutput(bool destroy = false)
        {
            if (destroy)
            {
                Destroy(currentOutput);
            }
            currentOutput = null;
        }
        
        
    }
}