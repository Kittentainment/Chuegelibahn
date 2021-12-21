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

    public TrackBuilder? currentTrackBuilder { get; private set; }

    public bool isGrabbed { get; private set; } = false;
    

    public void StartGrab()
    {
        if (isGrabbed) throw new InvalidOperationException("Draggable is already Grabbed!");

        currentTrackBuilder = new TrackBuilder(correspondingTrackPrinter.selectedType, correspondingTrackPrinter.transform.position, this.transform.position);
        isGrabbed = true;
    }

    private void FixedUpdate()
    {
        // TODO only for DEBUG, until we have input system and VR.
        if (isGrabbed)
        {
            DragToLocation(transform.position); 
        }
        else
        {
            StartGrab();
        }
    }

    public void DragToLocation(Vector3 newLocation)
    {
        transform.position = newLocation;
        if (isGrabbed == false || currentTrackBuilder == null)
        {
            throw new InvalidOperationException("Can't use DragToLocation if Draggable is not grabbed!");
        }
        else
        {
            currentTrackBuilder.OnDrag(correspondingTrackPrinter.transform, this.transform);
        }
    }

}