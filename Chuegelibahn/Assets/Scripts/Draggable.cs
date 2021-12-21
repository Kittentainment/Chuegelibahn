#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draggable : MonoBehaviour
{
    // ReSharper disable once InconsistentNaming
    [SerializeField] private TrackPrinter _correspondingTrackPrinter;
    public TrackPrinter correspondingTrackPrinter => _correspondingTrackPrinter;

    public TrackBuilder? currentTrack { get; private set; }
    
    public bool isGrabbed { get; private set; }



    public void Grab()
    {
        if (isGrabbed) throw new InvalidOperationException("Draggable is already Grabbed!");
        
        
    }
    
    public void DragToLocation(Vector3 newLocation)
    {
        transform.position = newLocation;
        if (isGrabbed == false)
        {
            throw new InvalidOperationException("Can't use DragToLocation if Draggable is not grabbed!");
        }
        else
        {
            CheckForTrackUpdate();
        }
    }

    private void CheckForTrackUpdate()
    {
        
    }
}
