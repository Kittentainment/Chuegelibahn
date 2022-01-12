using JetBrains.Annotations;
using UnityEngine;

namespace Copy
{
    public class CopyOutputLocation : MonoBehaviour
    {
        [CanBeNull] private Copyable _currentOutput;

        [CanBeNull]
        public Copyable currentOutput
        {
            get => _currentOutput;
            set
            {
                if (_currentOutput != null)
                {
                    Destroy(_currentOutput.gameObject);
                }
                _currentOutput = value;
            }
        }
    }
}