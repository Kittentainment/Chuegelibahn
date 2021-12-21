using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackPrefabs : MonoBehaviour
{
    public static TrackPrefabs instance { get; private set; }

    [SerializeField] private TrackPiece straightPiece;
    public TrackPiece StraightPiece => straightPiece;
    [SerializeField] private TrackPiece curvePiece;
    public TrackPiece CurvePiece => curvePiece;
    
    
    private void Awake()
    {
        if (instance != null)
        {
            throw new Exception("TrackPrefabs should only exist once!");
        }
        instance = this;
    }


    public static float GetLengthOfTrackPiece(TrackType type)
    {
        return type switch
        {
            TrackType.Straight => 1,
            TrackType.Left => 1,
            TrackType.Right => 1,
            TrackType.Up => 1,
            TrackType.Down => 1,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
    
}
