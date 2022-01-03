using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackPrinter : MonoBehaviour
{
    [SerializeField] private Draggable draggablePrefab; // TODO get the prefab in a more elegant way.
    
    public TrackType selectedType { get; set; } = TrackType.Straight;

    public Draggable draggable { get; private set; }

    private void Awake()
    {
        draggable = GameObject.Instantiate(draggablePrefab);
        draggable.transform.position = transform.position;
        draggable.trackPrinter = this;
    }

    public void OnSelected()
    {
        Debug.Log("Grabbed TrackPrinter");
    }
    
    public void OnDeselected()
    {
        Debug.Log("Let go of TrackPrinter");
    }


    public void OnActivate()
    {
        Debug.Log("TrackPrinter -- OnActivate");
        PrintCurrentTrack();
        // TODO: also let go of draggable immediately
        // GetComponent<XR>()
        // http://snapandplug.com/xr-input-toolkit-2020-faq/#FAQ:-Can-I-force-a-hand/controller-to-drop-held-items?
    }


    public void PrintCurrentTrack()
    {
        draggable.PrintCurrentTrack();
    }
    
}
