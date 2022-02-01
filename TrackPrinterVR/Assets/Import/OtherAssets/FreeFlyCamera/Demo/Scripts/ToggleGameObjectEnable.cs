using UnityEngine;
using UnityEngine.InputSystem;

public class ToggleGameObjectEnable : MonoBehaviour
{
    [SerializeField]
    private GameObject _go;

    private bool _enabled = true;

    private void Update()
    {
        if (Keyboard.current.hKey.wasPressedThisFrame)
        {
            _enabled = !_enabled;
            _go.SetActive(_enabled);
        }
    }
}
