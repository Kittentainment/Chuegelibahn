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

    public ITrackBuilder? currentTrackBuilder { get; private set; }
    
    public bool isGrabbed { get; private set; }



    public void StartGrab()
    {
        if (isGrabbed) throw new InvalidOperationException("Draggable is already Grabbed!");

        isGrabbed = true;
        switch (correspondingTrackPrinter.selectedType)
        {
            case TrackType.Straight:
                currentTrackBuilder = new StraightTrackBuilder(correspondingTrackPrinter.transform.position, this.transform.position);
                break;
            case TrackType.Left:
                break;
            case TrackType.Right:
                break;
            case TrackType.Up:
                break;
            case TrackType.Down:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void FixedUpdate()
    {
        if (isGrabbed)
        {
            DragToLocation(transform.position); // TODO only for DEBUG
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