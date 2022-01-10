using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class to switch the material of a MeshRenderer easily back and forth between the existing material and a new given material.
/// </summary>
[RequireComponent(typeof(MeshRenderer))]
public class MaterialSwitcher : MonoBehaviour
{
    public Material oldMaterial { get; private set; }
    
    public MeshRenderer meshRenderer { get; private set; }

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void SwitchToNewMaterial(Material newMaterial)
    {
        oldMaterial = meshRenderer.material;
        meshRenderer.material = newMaterial;
    }

    public void SwitchBack()
    {
        if (oldMaterial == null)
        {
            Debug.LogWarning("MaterialSwitcher::SwitchBack - nothing to switch to!");
            return;
        }

        meshRenderer.material = oldMaterial;
    }
    
}
