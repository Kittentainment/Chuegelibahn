using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackPrefabManager : MonoBehaviour
{
    public static TrackPrefabManager instance { get; private set; }

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
            TrackType.Straight => 0.01f,
            TrackType.Left => 0.1f,
            TrackType.Right => 0.1f,
            TrackType.Up => 0.1f,
            TrackType.Down => 0.1f,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
    
    public static Vector3 GetDefaultRotationOfPiece(TrackType type)
    {
        return new Vector3(-90, 0, 0);
    }
    
}
